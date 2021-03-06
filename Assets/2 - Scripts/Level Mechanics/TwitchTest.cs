using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using System.Net;
using System.Threading;
using System;
using System.Text;

public class TwitchTest : MonoBehaviour
{

	[SerializeField] private TwitchIRC _irc = null;
	UnityHttpListener _listen = null;

	public class UnityHttpListener
	{

		private HttpListener listener;
		private Thread listenerThread;
		public bool enabled = false;
		public string oAuth = "";
		public string login = "";

		public delegate void OnAuthReceived();
		public OnAuthReceived OnAuthReceivedEvent;

		public void Start()
		{
			if (listener != null && listener.IsListening) return;
			listener = new HttpListener();
			listener.Prefixes.Add("http://localhost/");
			listener.AuthenticationSchemes = AuthenticationSchemes.Anonymous;
			listener.Start();

			enabled = true;
			listenerThread = new Thread(startListener);
			listenerThread.Start();
			Debug.Log("Server Started");
		}

		~UnityHttpListener()
        {
			listener.Stop();
        }

		private void startListener()
		{
			while (enabled)
			{
				var result = listener.BeginGetContext(ListenerCallback, listener);
				result.AsyncWaitHandle.WaitOne();
			}
			listener.Stop();
		}

		private void ListenerCallback(IAsyncResult result)
		{
			var context = listener.EndGetContext(result);

			switch (context.Request.HttpMethod)
			{
				case "GET":
					if (context.Request.Url.LocalPath != "/") break;
					///	Respond to the localhost w/ a web page that makes an AJAX call passing along the user's access token.
					HttpListenerResponse response = context.Response;
					// Construct a response.
					byte[] buffer = Encoding.UTF8.GetBytes(File.ReadAllText(Application.streamingAssetsPath+"/authPage.html"));
					// Get a response stream and write the response to it.
					response.ContentLength64 = buffer.Length;
					Stream output = response.OutputStream;
					output.Write(buffer, 0, buffer.Length);
					// You must close the output stream.
					output.Close();
					break;
				case "POST":
					///	When the AJAX call is made, collect the access token
					Thread.Sleep(1000);
					var data_text = new StreamReader(context.Request.InputStream, context.Request.ContentEncoding).ReadToEnd();
					enabled = false;
					listener.Stop();
					oAuth = data_text.Split('|')[0].Split('=')[1];
					login = data_text.Split('|')[1];
					Debug.Log("login: " + login + " || oAuth: " + oAuth);
					OnAuthReceivedEvent?.Invoke();
					break;
			}
			context.Response.Close();
		}
	}

	public void OpenTwitch()
    {
		 _listen = new UnityHttpListener();
		_listen.OnAuthReceivedEvent -= _ConfigureIRC;
		_listen.OnAuthReceivedEvent += _ConfigureIRC;
		_listen.Start();
        Application.OpenURL("https://id.twitch.tv/oauth2/authorize?response_type=token&client_id=7eebwhs11hheq8ilzp1r2uz69n8fv0&redirect_uri=http://localhost&scope=chat:read%20chat:edit%20channel:read:redemptions%20channel:read:subscriptions&force_verify=true");
    }

    private void Update()
    {
        if (_irc.twitchDetails.oauth == "" && Input.GetKeyDown(KeyCode.L)) OpenTwitch();
		if (_irc.twitchDetails.oauth != "" && !_irc.enabled) _irc.enabled = true;
    }

    private void _ConfigureIRC()
	{
		_irc.newChatMessageEvent.AddListener(_OnMessageReceived);
		_irc.twitchDetails.oauth = _listen.oAuth;
		_irc.twitchDetails.nick = _listen.login;
		_irc.twitchDetails.channel = _listen.login;
	}

	private void _OnMessageReceived(Chatter _pChatter)
    {
		Debug.Log(_pChatter.login + ":\t" + _pChatter.message);
		if (_pChatter.message == "!Quit")
        {
			_irc.IRC_Disconnect();
        }
    }

}