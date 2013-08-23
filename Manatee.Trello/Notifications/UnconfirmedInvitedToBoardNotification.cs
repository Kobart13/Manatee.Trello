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
 
	File Name:		UnconfirmedInvitedToBoardNotification.cs
	Namespace:		Manatee.Trello
	Class Name:		UnconfirmedInvitedToBoardNotification
	Purpose:		Provides notification that an unconfirmed member was invited
					to a board.

***************************************************************************************/
using System.Diagnostics;

namespace Manatee.Trello
{
	/// <summary>
	/// Provides notification that an unconfirmed member was invited to a board.
	/// </summary>
	public class UnconfirmedInvitedToBoardNotification : Notification
	{
		/// <summary>
		/// Creates a new instance of the UnconfirmedInvitedToBoardNotification class.
		/// </summary>
		/// <param name="notification">The base notification</param>
		public UnconfirmedInvitedToBoardNotification(Notification notification)
			: base(notification.Svc, notification.Id)
		{
			Debug.Assert(false, string.Format(
				"{0} is not yet configured.  Please post the JSON returned by http://api.trello.com/notifications/{1} to https://trello.com/c/NX8xPcP6",
				GetType().Name, notification.Id));
			VerifyNotExpired();
		}
	}
}