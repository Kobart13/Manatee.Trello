﻿/***************************************************************************************

	Copyright 2013 Little Crab Solutions

	   Licensed under the Apache License, Version 2.0 (the "License");
	   you may not use this file except in compliance with the License.
	   You may obtain a copy of the License at

		 http://www.apache.org/licenses/LICENSE-2.0

	   Unless required by applicable law or agreed to in writing, software
	   distributed under the License is distributed on an "AS IS" BASIS,
	   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
	   See the License for the specific language governing permissions and
	   limitations under the License.
 
	File Name:		Organization.cs
	Namespace:		Manatee.Trello
	Class Name:		Organization
	Purpose:		Represents an organization on Trello.com.

***************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using Manatee.Trello.Contracts;
using Manatee.Trello.Internal;
using Manatee.Trello.Internal.Json;
using Manatee.Trello.Json;
using Manatee.Trello.Rest;

namespace Manatee.Trello
{
	/// <summary>
	/// Represents an organization.
	/// </summary>
	public class Organization : ExpiringObject, IEquatable<Organization>, IComparable<Organization>
	{
		private IJsonOrganization _jsonOrganization;
		private readonly ExpiringList<Action, IJsonAction> _actions;
		private readonly ExpiringList<Board, IJsonBoard> _boards;
		private readonly ExpiringList<InvitedMember, IJsonMember> _invitedMembers;
		private readonly ExpiringList<Member, IJsonMember> _members;
		private readonly ExpiringList<OrganizationMembership, IJsonOrganizationMembership> _memberships;
		private readonly OrganizationPreferences _preferences;
		private bool _isDeleted;

		///<summary>
		/// Enumerates all actions associated with this organization.
		///</summary>
		public IEnumerable<Action> Actions { get { return _isDeleted ? Enumerable.Empty<Action>() : _actions; } }
		/// <summary>
		/// Enumerates the boards owned by the organization.
		/// </summary>
		public IEnumerable<Board> Boards { get { return _isDeleted ? Enumerable.Empty<Board>() : _boards; } }
		/// <summary>
		/// Gets or sets the description for the organization.
		/// </summary>
		public string Description
		{
			get
			{
				if (_isDeleted) return null;
				VerifyNotExpired();
				return (_jsonOrganization == null) ? null : _jsonOrganization.Desc;
			}
			set
			{
				if (_isDeleted) return;
				Validator.Writable();
				if (_jsonOrganization == null) return;
				if (_jsonOrganization.Desc == value) return;
				_jsonOrganization.Desc = value ?? string.Empty;
				Parameters.Add("desc", _jsonOrganization.Desc);
				Put();
			}
		}
		/// <summary>
		/// Gets or sets the name to be displayed for the organization.
		/// </summary>
		public string DisplayName
		{
			get
			{
				if (_isDeleted) return null;
				VerifyNotExpired();
				return (_jsonOrganization == null) ? null : _jsonOrganization.DisplayName;
			}
			set
			{
				if (_isDeleted) return;
				Validator.Writable();
				if (_jsonOrganization == null) return;
				if (_jsonOrganization.DisplayName == value) return;
				_jsonOrganization.DisplayName = Validator.MinStringLength(value, 4, "DisplayName");
				Parameters.Add("displayName", _jsonOrganization.DisplayName);
				Put();
			}
		}
		/// <summary>
		/// Gets a unique identifier (not necessarily a GUID).
		/// </summary>
		public override string Id
		{
			get { return _jsonOrganization != null ? _jsonOrganization.Id : base.Id; }
			internal set
			{
				if (_jsonOrganization != null)
					_jsonOrganization.Id = value;
				base.Id = value;
			}
		}
		/// <summary>
		/// Enumerates all members who have received invitations to this organization.
		/// </summary>
		public IEnumerable<Member> InvitedMembers { get { return _invitedMembers; } }
		/// <summary>
		/// Gets whether this organization has paid features.
		/// </summary>
		internal bool? IsPaidAccount { get { return (_jsonOrganization == null) ? null : _jsonOrganization.PaidAccount; } }
		/// <summary>
		/// Gets the organization's logo hash.
		/// </summary>
		public string LogoHash
		{
			get
			{
				if (_isDeleted) return null;
				VerifyNotExpired();
				return (_jsonOrganization == null) ? null : _jsonOrganization.LogoHash;
			}
		}
		/// <summary>
		/// Enumerates the members who belong to the organization.
		/// </summary>
		public IEnumerable<OrganizationMembership> Memberships { get { return _isDeleted ? Enumerable.Empty<OrganizationMembership>() : _memberships; } }
		/// <summary>
		/// Gets or sets the name of the organization.
		/// </summary>
		public string Name
		{
			get
			{
				if (_isDeleted) return null;
				VerifyNotExpired();
				return (_jsonOrganization == null) ? null : _jsonOrganization.Name;
			}
			set
			{
				if (_isDeleted) return;
				Validator.Writable();
				if (_jsonOrganization == null) return;
				if (_jsonOrganization.Name == value) return;
				_jsonOrganization.Name = Validator.OrgName(value);
				Parameters.Add("name", _jsonOrganization.Name);
				Put();
			}
		}
		/// <summary>
		/// Enumerates the powerups obtained by the organization.
		/// </summary>
		public IEnumerable<int> PowerUps
		{
			get
			{
				if (_isDeleted) return Enumerable.Empty<int>();
				VerifyNotExpired();
				return (_jsonOrganization == null) ? null : _jsonOrganization.PowerUps;
			}
		}
		///<summary>
		/// Gets the set of preferences for the organization.
		///</summary>
		public OrganizationPreferences Preferences { get { return _isDeleted ? null : _preferences; } }
		/// <summary>
		/// Gets a collection of premium features available to the organization.
		/// </summary>
		public IEnumerable<string> PremiumFeatures
		{
			get
			{
				if (_isDeleted) return Enumerable.Empty<string>();
				VerifyNotExpired();
				return (_jsonOrganization == null) ? null : _jsonOrganization.PremiumFeatures;
			}
		}
		/// <summary>
		/// Gets the URL to the organization's profile.
		/// </summary>
		public string Url { get { return (_jsonOrganization == null) ? null : _jsonOrganization.Url; } }
		/// <summary>
		/// Gets or sets the organization's website.
		/// </summary>
		public string Website
		{
			get
			{
				if (_isDeleted) return null;
				VerifyNotExpired();
				return (_jsonOrganization == null) ? null : _jsonOrganization.Website;
			}
			set
			{
				if (_isDeleted) return;
				Validator.Writable();
				if (_jsonOrganization == null) return;
				if (_jsonOrganization.Website == value) return;
				_jsonOrganization.Website = value ?? string.Empty;
				Parameters.Add("website", _jsonOrganization.Website);
				Put();
			}
		}

		internal static string TypeKey { get { return "organizations"; } }
		internal static string TypeKey2 { get { return "organizations"; } }
		internal override string PrimaryKey { get { return TypeKey; } }
		internal override string SecondaryKey { get { return TypeKey2; } }

		/// <summary>
		/// Creates a new instance of the Organization class.
		/// </summary>
		public Organization()
		{
			_jsonOrganization = new InnerJsonOrganization();
			_actions = new ExpiringList<Action, IJsonAction>(this, Action.TypeKey) {Fields = "id"};
			_boards = new ExpiringList<Board, IJsonBoard>(this, Board.TypeKey) {Fields = "id"};
			_invitedMembers = new ExpiringList<InvitedMember, IJsonMember>(this, InvitedMember.TypeKey) {Fields = "id"};
			_members = new ExpiringList<Member, IJsonMember>(this, Member.TypeKey) {Fields = "id"};
			_memberships = new ExpiringList<OrganizationMembership, IJsonOrganizationMembership>(this, OrganizationMembership.TypeKey);
			_preferences = new OrganizationPreferences(this);
		}

		///<summary>
		/// Adds an existing member to the organization or updates the permissions of a member already in the organization.
		///</summary>
		///<param name="member">The member</param>
		///<param name="type">The permission level for the member</param>
		public void AddOrUpdateMember(Member member, OrganizationMembershipType type = OrganizationMembershipType.Normal)
		{
			if (Svc == null) return;
			if (_isDeleted) return;
			Validator.Writable();
			Validator.Entity(member);
			var endpoint = EndpointGenerator.Default.Generate(this, member);
			Parameters.Add("type", type.ToLowerString());
			JsonRepository.Put<IJsonMember>(endpoint.ToString(), Parameters);
			Parameters.Clear();
			_members.MarkForUpdate();
			_memberships.MarkForUpdate();
			_actions.MarkForUpdate();
		}
		/// <summary>
		///  Adds a new or existing member to the organization or updates the permissions of a member already in the organization.
		/// </summary>
		/// <param name="fullName"></param>
		/// <param name="type">The permission level for the member</param>
		/// <param name="emailAddress"></param>
		public Member AddOrUpdateMember(string emailAddress, string fullName, OrganizationMembershipType type = OrganizationMembershipType.Normal)
		{
			if (Svc == null) return null;
			if (_isDeleted) return null;
			Validator.Writable();
			Validator.NonEmptyString(emailAddress);
			Validator.NonEmptyString(fullName);
			var member = Svc.SearchMembers(emailAddress, 1).FirstOrDefault();
			if (member != null)
			{
				AddOrUpdateMember(member, type);
				_members.MarkForUpdate();
				_memberships.MarkForUpdate();
				_actions.MarkForUpdate();
			}
			return member;
		}
		/// <summary>
		/// Creates a board for the organization, owned by the current member.
		/// </summary>
		/// <param name="name">The name of the board.</param>
		/// <returns>The newly-created Board object.</returns>
		public Board CreateBoard(string name)
		{
			if (Svc == null) return null;
			Validator.Writable();
			Validator.NonEmptyString(name);
			var board = new Board();
			var endpoint = EndpointGenerator.Default.Generate(board);
			Parameters.Add("name", name);
			Parameters.Add("idOrganization", Id);
			board.ApplyJson(JsonRepository.Post<IJsonBoard>(endpoint.ToString(), Parameters));
			Parameters.Clear();
			UpdateService(board);
			_boards.MarkForUpdate();
			return board;
		}
		/// <summary>
		/// Deletes the organization.  This cannot be undone.
		/// </summary>
		public void Delete()
		{
			if (Svc == null) return;
			if (_isDeleted) return;
			Validator.Writable();
			if (_members != null)
			{
				foreach (var member in _members)
				{
					member.OrganizationsList.MarkForUpdate();
				}
			}
			var endpoint = EndpointGenerator.Default.Generate(this);
			JsonRepository.Delete<IJsonOrganization>(endpoint.ToString());
			_isDeleted = true;
		}
		/// <summary>
		/// Extends an invitation to the organization to another member.
		/// </summary>
		/// <param name="member">The member to invite.</param>
		/// <param name="type">The level of membership offered.</param>
		internal void InviteMember(Member member, BoardMembershipType type = BoardMembershipType.Normal)
		{
			if (Svc == null) return;
			if (_isDeleted) return;
			Validator.Writable();
			Validator.Entity(member);
			Log.Error(new NotSupportedException("Inviting members to organizations is not yet supported by the Trello API."));
		}
		///<summary>
		/// Removes a member from the organization.
		///</summary>
		///<param name="member"></param>
		public void RemoveMember(Member member)
		{
			if (Svc == null) return;
			if (_isDeleted) return;
			Validator.Writable();
			Validator.Entity(member);
			var endpoint = EndpointGenerator.Default.Generate(this, member);
			JsonRepository.Delete<IJsonOrganization>(endpoint.ToString());
			_members.MarkForUpdate();
			_memberships.MarkForUpdate();
			_actions.MarkForUpdate();
		}
		/// <summary>
		/// Rescinds an existing invitation to the organization.
		/// </summary>
		/// <param name="member"></param>
		internal void RescindInvitation(Member member)
		{
			if (Svc == null) return;
			if (_isDeleted) return;
			Validator.Writable();
			Validator.Entity(member);
			Log.Error(new NotSupportedException("Inviting members to organizations is not yet supported by the Trello API."));
		}
		/// <summary>
		/// Indicates whether the current object is equal to another object of the same type.
		/// </summary>
		/// <returns>
		/// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
		/// </returns>
		/// <param name="other">An object to compare with this object.</param>
		public bool Equals(Organization other)
		{
			return Id == other.Id;
		}
		/// <summary>
		/// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
		/// </summary>
		/// <returns>
		/// true if the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>; otherwise, false.
		/// </returns>
		/// <param name="obj">The object to compare with the current object. </param><filterpriority>2</filterpriority>
		public override bool Equals(object obj)
		{
			if (!(obj is Organization)) return false;
			return Equals((Organization) obj);
		}
		/// <summary>
		/// Serves as a hash function for a particular type. 
		/// </summary>
		/// <returns>
		/// A hash code for the current <see cref="T:System.Object"/>.
		/// </returns>
		/// <filterpriority>2</filterpriority>
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
		/// <summary>
		/// Compares the current object with another object of the same type.
		/// </summary>
		/// <returns>
		/// A value that indicates the relative order of the objects being compared. The return value has the following meanings: Value Meaning Less than zero This object is less than the <paramref name="other"/> parameter.Zero This object is equal to <paramref name="other"/>. Greater than zero This object is greater than <paramref name="other"/>. 
		/// </returns>
		/// <param name="other">An object to compare with this object.</param>
		public int CompareTo(Organization other)
		{
			var order = string.Compare(DisplayName, other.DisplayName);
			return order;
		}
		/// <summary>
		/// Returns a string that represents the current object.
		/// </summary>
		/// <returns>
		/// A string that represents the current object.
		/// </returns>
		/// <filterpriority>2</filterpriority>
		public override string ToString()
		{
			return DisplayName;
		}
		/// <summary>
		/// Retrieves updated data from the service instance and refreshes the object.
		/// </summary>
		public override bool Refresh()
		{
			var endpoint = EndpointGenerator.Default.Generate(this);
			Parameters.Add("fields", "name,displayName,desc,invited,powerUps,url,website,logoHash,premiumFeatures");
			Parameters.Add("paid_account", "true");
			Parameters.Add("actions", "none");
			Parameters.Add("members", "none");
			Parameters.Add("membersInvited", "none");
			Parameters.Add("boards", "none");
			Parameters.Add("memberships", "none");
			var obj = JsonRepository.Get<IJsonOrganization>(endpoint.ToString(), Parameters);
			Parameters.Clear();
			if (obj == null) return false;
			ApplyJson(obj);
			return true;
		}

		/// <summary>
		/// Propagates the service instance to the object's owned objects.
		/// </summary>
		protected override void PropagateService()
		{
			UpdateService(_actions);
			UpdateService(_boards);
			UpdateService(_members);
			UpdateService(_memberships);
			UpdateService(_preferences);
		}

		internal override void ApplyJson(object obj)
		{
			if (obj is IRestResponse)
				_jsonOrganization = ((IRestResponse<IJsonOrganization>)obj).Data;
			else
				_jsonOrganization = (IJsonOrganization)obj;
		}
		internal override bool Matches(string id)
		{
			return (Id == id) || (Name == id);
		}

		private void Put()
		{
			var endpoint = EndpointGenerator.Default.Generate(this);
			JsonRepository.Put<IJsonOrganization>(endpoint.ToString(), Parameters);
			Parameters.Clear();
			_actions.MarkForUpdate();
		}
	}
}