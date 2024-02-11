using ServidorChatAlexMan;
using System.Net;
using System.Text;

class Program
{
    const int HTTP_TOT_OK = 200;
    const int HTTP_ERROR = 500;
    const string URL_SERVIDOR = "http://localhost:51111/";

    private static userChat chatServer;

    static async Task Main(string[] args)
    {
        chatServer = new userChat();
        await ListenAsync(chatServer);
        Console.ReadLine();
    }

    private static async Task ListenAsync(userChat chatServer)
    {
        HttpListener listener = new HttpListener();
        listener.Prefixes.Add(URL_SERVIDOR);
        listener.Start();

        try
        {
            while (true)
            {
                HttpListenerContext context = null;

                try
                {
                    context = await listener.GetContextAsync();
                    string url = context.Request.Url.Segments.LastOrDefault()?.Trim('/').ToLower();

                    if (url.StartsWith("registrarusuari"))
                    {
                        string usuari = context.Request.QueryString.Get("u");
                        chatServer.Users.Add(usuari);
                        await EscriureRespostaAsync(context, usuari, HTTP_TOT_OK);
                    }
                    else if (url.StartsWith("enviar"))
                    {
                        string usuariDesti = context.Request.QueryString.Get("u");
                        string usuariEmissor = context.Request.QueryString.Get("ue");
                        string missatge = context.Request.QueryString.Get("m");

                        chatServer.saveMessage(usuariDesti, usuariEmissor, missatge);
                        await EscriureRespostaAsync(context, missatge, HTTP_TOT_OK);
                    }
                    else if (url.StartsWith("rebre"))
                    {
                        string usuari = context.Request.QueryString.Get("u");
                        var missatgesRecents = chatServer.reciveMessage(usuari);
                        string resposta = string.Join("; ", missatgesRecents);
                        await EscriureRespostaAsync(context, resposta, HTTP_TOT_OK);
                    }
                    else if (url.StartsWith("rperdatahora"))
                    {
                        string usuari = context.Request.QueryString.Get("u");
                        string dataHoraString = context.Request.QueryString.Get("t");

                        if (DateTime.TryParseExact(dataHoraString, "dd/MM/yyyy H", null, System.Globalization.DateTimeStyles.None, out DateTime dataHora))
                        {
                            var llistaMissatges = chatServer.reciveMessageTime(usuari, dataHora);
                            string resposta = string.Join("; ", llistaMissatges);
                            await EscriureRespostaAsync(context, resposta, HTTP_TOT_OK);
                        }
                        else
                        {
                            await EscriureRespostaAsync(context, "Format de data/hora incorrecte", HTTP_ERROR);
                        }
                    }
                    else if (url.StartsWith("llistarusuaris"))
                    {
                        var llistaUsuaris = chatServer.usersList();
                        string resposta = string.Join("; ", llistaUsuaris);
                        await EscriureRespostaAsync(context, resposta, HTTP_TOT_OK);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processant la solicitud: {ex.Message}");
                    Console.WriteLine(ex.StackTrace);
                    await EscriureRespostaAsync(context, $"Error intern del servidor: {ex.Message}", HTTP_ERROR);
                }
                finally
                {
                    if (context != null) context.Response.Close();
                }
            }
        }
        finally
        {
            listener.Stop();
        }
    }

    private static async Task EscriureRespostaAsync(HttpListenerContext context, string msg, int codiError)
    {
        context.Response.ContentLength64 = Encoding.UTF8.GetByteCount(msg);
        context.Response.StatusCode = (int)HttpStatusCode.OK;
        using (Stream s = context.Response.OutputStream)
        using (StreamWriter writer = new StreamWriter(s))
            await writer.WriteAsync(msg);
    }

}