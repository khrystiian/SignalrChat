using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;
using SignalRApp.App_Data;
using SignalrChat.Models;

namespace SignalrChat.Hubs
{
    /// <summary>
    ///  A basic MVC application which exercises a chat functionality. 
    ///  Each user can edit his profile information together with the image profile which are saved in MSSQL database.
    ///  The application allows multiple users to connect and send messages to themselves or others via Signalr Hub.
    ///  The messages are being saved in the memory and can be read until the application is stopped.
    /// </summary>
    public class ChatHub : Hub
    {
        static string _SelectedPeer;
        UserRepo repo = new UserRepo();
        UserHelper helper = new UserHelper();
        public static List<User> ConnectedUsers = new List<User>();
        static List<Message> AllMessages = new List<Message>();

        public override Task OnConnected()
        {
            List<Message> messages = new List<Message>();
            try
            {
                var id = Context.ConnectionId;
                var username = Context.User.Identity.Name;
                var me = repo.GetUser(username);
                var imagePath = me.ImagePath;
                var base64Image = helper.LoadImage(imagePath);

                messages = (username.Trim() == _SelectedPeer.Trim()) ? AllMessages.Where(x => x.ToUsername == username && x.FromUsername == username)
                                                                                .OrderBy(x => x.DateTime)
                                                                                .ToList()
                                                                     : AllMessages.Where(x => x.ToUsername == username && x.FromUsername == _SelectedPeer)
                                                                                .Union(AllMessages.Where(x => x.FromUsername == username && x.ToUsername == _SelectedPeer))
                                                                                .OrderBy(x => x.DateTime)
                                                                                .ToList();                  

                if (ConnectedUsers.Count(x => x.Name == username) == 0)
                {
                    var user = new User {ConnectionId = id, Name = username, Base64Image = base64Image, Messages = messages };
                    ConnectedUsers.Add(user);
                }
                else
                {
                    foreach (var user in ConnectedUsers)
                    {
                        user.Messages = (user.Name == username) ? messages : new List<Message>();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return base.OnConnected();
        }

        public static void SelectedPeer(string name = null)
        {
            _SelectedPeer = name;
        }

        public void Send(string peerUsrName, string message)
        {
            var peerConId = ConnectedUsers.Find(x => x.Name == peerUsrName).ConnectionId;
            var myConId = Context.ConnectionId;

            if (message.Contains("<script>"))
                throw new HubException("This message will flow to the client", new { user = Context.User.Identity.Name, message });

            if (peerConId != null && myConId != null && message != null)
            {
                SaveMsgToCache(Context.User.Identity.Name, peerUsrName, message, DateTime.Now.ToString());
                
                Clients.Caller.message("You", message);
                Clients.Client(peerConId).message(Context.User.Identity.Name, message);
            }
        }
        public List<User> GetConnectedUsersWithMessages()
        {
            return ConnectedUsers;
        }

        public static User GetConnectionIdByName(string usrname)
        {
            return ConnectedUsers.FirstOrDefault(x => x.Name == usrname);
        }

        private void SaveMsgToCache(string fromUsername, string toUsername, string message, string datetime)
        {
            AllMessages.Add(new Message
            {
                FromUsername = fromUsername,
                ToUsername = toUsername,
                Msg = message,
                DateTime = datetime
            });
            if (AllMessages.Count > 1000)
                AllMessages.RemoveAt(0);
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            var user = ConnectedUsers.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
            if (user != null)
                ConnectedUsers.Remove(user);
            else
                Debug.WriteLine(String.Format("Client {0} timed out .", Context.ConnectionId));

            return base.OnDisconnected(stopCalled);
        }
    }
}