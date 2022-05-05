using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using jDevES.FirebaseIDToken;

namespace jDevES.Auth {
    public class jDevAuth {
        public static async Task<IDToken> OpenPopup(CancellationToken cancellationToken) {
            ClientWebSocket ws = new ClientWebSocket();
            await ws.ConnectAsync(new Uri("wss://relay.jdev.com.es/"), cancellationToken);

            string bwscode = Guid.NewGuid().ToString();
            await SendAsync(ws, "{\"clientID\":\"" + Guid.NewGuid() + "\",\"listens\":[\"jdev-es:auth/" + bwscode + "\"]}");

            UnityEngine.Application.OpenURL("https://jdev.com.es/auth?bwscode=" + bwscode);

            ArraySegment<byte> buffer = new ArraySegment<byte>(new byte[2048]);
            var result = await ws.ReceiveAsync(buffer, cancellationToken);

            await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "", cancellationToken);
            ws.Dispose();
            if (result.Count != 0 || result.CloseStatus == WebSocketCloseStatus.Empty) {
                string text = Encoding.UTF8.GetString(buffer.Array);
                var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(text);
                if (dict.ContainsKey("id_token")) {
                    return new IDToken(dict["id_token"], "jdev-es");
                }
            }

            return null;
        }

        private static Task SendAsync(ClientWebSocket ws, string str) {
            byte[] bytes = Encoding.UTF8.GetBytes(str);
            ArraySegment<byte> arr = new ArraySegment<byte>(bytes);
            return ws.SendAsync(arr, WebSocketMessageType.Text, true, CancellationToken.None);
        }
    } 
}