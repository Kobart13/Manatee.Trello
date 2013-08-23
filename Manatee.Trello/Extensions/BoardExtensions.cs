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
 
	File Name:		BoardExtensions.cs
	Namespace:		Manatee.Trello
	Class Name:		BoardExtensions
	Purpose:		Exposes extension methods for the Board entity.

***************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Manatee.Trello.Internal;
using Manatee.Trello.Json;

namespace Manatee.Trello
{
	/// <summary>
	/// Exposes extension methods for the Board entity.
	/// </summary>
	public static class BoardExtensions
	{
		/// <summary>
		/// Retrieves all actions on a board or its contents performed by a specified member.
		/// </summary>
		/// <param name="board">The board.</param>
		/// <param name="member">The member.</param>
		/// <returns>A collection of actions.</returns>
		public static IEnumerable<Action> ActionsByMember(this Board board, Member member)
		{
			// It doesn't appear that the API supports this call.  This makes me sad.   <:-(
			//var response = Enumerable.Empty<IJsonAction>();
			//if (board.Svc != null)
			//{
			//    var endpoint = EndpointGenerator.Default.Generate(board);
			//    endpoint.Append(Action.TypeKey);
			//    var request = board.Svc.RestClientProvider.RequestProvider.Create(endpoint.ToString());
			//    request.AddParameter("member", member.Id);
			//    response = board.Svc.Api.Get<List<IJsonAction>>(request);
			//}
			//foreach (var jsonAction in response)
			//{
			//    var action = new Action();
			//    action.ApplyJson(jsonAction);
			//    yield return ActionProvider.Default.Parse(action);
			//}
			return board.Actions.Where(a => a.MemberCreator.Equals(member));
		}
		/// <summary>
		/// Retrieves all members of the board who have Admin privileges.
		/// </summary>
		/// <param name="board">The board.</param>
		/// <returns>A collection of members.</returns>
		public static IEnumerable<Member> Admins(this Board board)
		{
			if (board.Svc != null)
			{
				var endpoint = EndpointGenerator.Default.GenerateForList<Member>(board);
				var parameters = new Dictionary<string, object>
					{
						{"fields", "id"},
						{"filter", "admins"},
					};
				var response = board.JsonRepository.Get<List<IJsonMember>>(endpoint.ToString(), parameters);
				return response.Select(j => board.Svc.Retrieve<Member>(j.Id));
			}
			return Enumerable.Empty<Member>();
		}
		/// <summary>
		/// Retrieves all cards contained within a board, both archived and active.
		/// </summary>
		/// <param name="board">The board.</param>
		/// <returns>A collection of cards.</returns>
		public static IEnumerable<Card> AllCards(this Board board)
		{
			if (board.Svc != null)
			{
				var endpoint = EndpointGenerator.Default.GenerateForList<Card>(board);
				var parameters = new Dictionary<string, object>
					{
						{"fields", "id"},
						{"filter", "all"},
						{"actions", "none"},
						{"attachments", "false"},
						{"badges", "false"},
						{"members", "false"},
						{"membersVoted", "false"},
						{"checkItemStates", "false"},
						{"checkLists", "false"},
						{"board", "false"},
						{"list", "false"},
					};
				var response = board.JsonRepository.Get<List<IJsonCard>>(endpoint.ToString(), parameters);
				return response.Select(j => board.Svc.Retrieve<Card>(j.Id));
			}
			return Enumerable.Empty<Card>();
		}
		/// <summary>
		/// Retrieves all lists contained within a board, both archived and active.
		/// </summary>
		/// <param name="board">The board.</param>
		/// <returns>A collection of lists.</returns>
		public static IEnumerable<List> AllLists(this Board board)
		{
			if (board.Svc != null)
			{
				var endpoint = EndpointGenerator.Default.GenerateForList<List>(board);
				var parameters = new Dictionary<string, object>
					{
						{"fields", "id"},
						{"filter", "all"},
						{"cards", "none"},
					};
				var response = board.JsonRepository.Get<List<IJsonList>>(endpoint.ToString(), parameters);
				return response.Select(j => board.Svc.Retrieve<List>(j.Id));
			}
			return Enumerable.Empty<List>();
		}
		/// <summary>
		/// Retrieves all active cards contained within a board.
		/// </summary>
		/// <param name="board">The board.</param>
		/// <returns>A collection of cards.</returns>
		public static IEnumerable<Card> Cards(this Board board)
		{
			if (board.Svc != null)
			{
				var endpoint = EndpointGenerator.Default.GenerateForList<Card>(board);
				var parameters = new Dictionary<string, object>
					{
						{"fields", "id"},
						{"filter", "visible"},
						{"actions", "none"},
						{"attachments", "false"},
						{"badges", "false"},
						{"members", "false"},
						{"membersVoted", "false"},
						{"checkItemStates", "false"},
						{"checkLists", "false"},
						{"board", "false"},
						{"list", "false"},
					};
				var response = board.JsonRepository.Get<List<IJsonCard>>(endpoint.ToString(), parameters);
				return response.Select(j => board.Svc.Retrieve<Card>(j.Id));
			}
			return Enumerable.Empty<Card>();
		}
		/// <summary>
		/// Retrieves all active cards contained within a board which are assigned to a specified member.
		/// </summary>
		/// <param name="board">The board.</param>
		/// <param name="member">The member.</param>
		/// <returns>A collection of cards.</returns>
		public static IEnumerable<Card> CardsAssignedToMember(this Board board, Member member)
		{
			return board.Cards().Where(c => c.Members.Contains(member));
		}
		/// <summary>
		/// Retrieves all active cards within a board with due dates within a specified time span of DateTime.Now.
		/// </summary>
		/// <param name="board">The board.</param>
		/// <param name="timeSpan">The time span.</param>
		/// <returns>A collection of cards.</returns>
		public static IEnumerable<Card> CardsDueSoon(this Board board, TimeSpan timeSpan)
		{
			return board.Cards().Where(c => c.DueDate - timeSpan <= DateTime.Now);
		}
		/// <summary>
		/// Retrieves all active cards within a board with names or descriptions which match a specified Regex.
		/// </summary>
		/// <param name="board">The board.</param>
		/// <param name="regex">The Regex.</param>
		/// <returns>A collection of cards.</returns>
		/// <remarks>Description matching does not account for Markdown syntax.</remarks>
		public static IEnumerable<Card> CardsMatching(this Board board, Regex regex)
		{
			return board.Cards().Where(c => regex.IsMatch(c.Description) || regex.IsMatch(c.Name));
		}
		/// <summary>
		/// Retrieves all active cards within a board with names or descriptions which have a specified label color applied.
		/// </summary>
		/// <param name="board">The board.</param>
		/// <param name="color">The label color.</param>
		/// <returns>A collection of cards.</returns>
		public static IEnumerable<Card> CardsWithLabel(this Board board, LabelColor color)
		{
			return board.Cards().Where(c => c.Labels.Any(l => l.Color == color));
		}
		/// <summary>
		/// Retrieves all members of a board without specifying their permission level.
		/// </summary>
		/// <param name="board">The board.</param>
		/// <returns>A collection of members.</returns>
		public static IEnumerable<Member> Members(this Board board)
		{
			return board.Memberships.Select(m => m.Member);
		}
	}
}