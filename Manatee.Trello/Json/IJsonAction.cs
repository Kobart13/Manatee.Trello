using System;
using System.Collections.Generic;

namespace Manatee.Trello.Json
{
	/// <summary>
	/// Defines the JSON structure for the Action object.
	/// </summary>
	public interface IJsonAction : IJsonCacheable
	{
		/// <summary>
		/// Gets or sets the ID of the member who performed the action.
		/// </summary>
		[JsonDeserialize]
		IJsonMember MemberCreator { get; set; }
		/// <summary>
		/// Gets or sets the data associated with the action.  Contents depend upon the action's type.
		/// </summary>
		[JsonDeserialize]
		IJsonActionData Data { get; set; }
		/// <summary>
		/// Gets or sets the action's type.
		/// </summary>
		[JsonDeserialize]
		ActionType? Type { get; set; }
		///<summary>
		/// Gets or sets the date on which the action was performed.
		///</summary>
		[JsonDeserialize]
		DateTime? Date { get; set; }

		/// <summary>
		/// Gets or sets the text for a comment while updating it.
		/// </summary>
		[JsonSerialize]
		string Text { get; set; }
		/// <summary>
		/// Gets or sets the list of reactions for comment actions.
		/// </summary>
		[JsonDeserialize]
		List<IJsonCommentReaction> Reactions { get; set; }
	}
}