using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServidorChatAlexMan
{
    public class userChat
    {

        private Dictionary<string, List<string>> userMessages;
        private List<string> users;

        public List<string> Users { get => users; set => users = value; }

        public userChat()
        {
            userMessages = new Dictionary<string, List<string>>();
            this.users = new List<string>();
        }

        public void saveMessage(string destinationUser, string sendUser, string message)
        {
            if (!userMessages.ContainsKey(destinationUser)) userMessages[destinationUser] = new List<string>();

            if (!userMessages.ContainsKey(sendUser)) userMessages[sendUser] = new List<string>();

            userMessages[sendUser].Add($"Enviat: a {destinationUser} ({DateTime.Now}): {message}");
            userMessages[destinationUser].Add($"Rebut: de {sendUser} ({DateTime.Now}): {message}");
        }

        public List<string> reciveMessage(string nomUsuari)
        {
            // Retorna els missatges de l'usuari o una llista buida si l'usuari no existeix
            return userMessages.ContainsKey(nomUsuari) ? userMessages[nomUsuari] : new List<string>();
        }

        public List<string> reciveMessageTime(string nomUsuari, DateTime dataIHora)
        {
            // Retorna els missatges posteriors a la data/hora proporcionada
            List<string> missatgesPosteriors = new List<string>();

            if (userMessages.ContainsKey(nomUsuari))
            {
                foreach (var missatge in userMessages[nomUsuari])
                {
                    // Obté la data/hora del missatge (suposant que el format és correcte)

                    var parts = missatge.Split(' ');
                    var data = parts[1].Trim('(');
                    var hora = parts[2].Split(':');

                    StringBuilder sb = new StringBuilder(data);
                    sb.Append(' ');
                    sb.Append(hora[0]);

                    if (DateTime.TryParseExact(sb.ToString(), "dd/MM/yyyy H", CultureInfo.InvariantCulture, DateTimeStyles.None, out var dataMissatge))
                    {
                        if (dataMissatge == dataIHora) missatgesPosteriors.Add(missatge);
                    }
                }
            }

            return missatgesPosteriors;
        }

        public List<string> usersList()
        {
            return this.Users;
        }


    }
}
