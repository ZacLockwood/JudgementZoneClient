using System;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson;

namespace JudgementZone.Models
{
    public class M_ChatMessage
    {
        [BsonId(IdGenerator = typeof(CombGuidGenerator))]
        public Guid UUId { get; set; } = Guid.NewGuid();

        [BsonElement("Username")]
        public string Username { get; set; }

        [BsonElement("Message")]
        public string Message { get; set; }

        #region Constructors

        public M_ChatMessage()
        {
            
        }

        public M_ChatMessage(string username, string message)
        {
            if (String.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentNullException(nameof(username), "Argument must not be null or white space.");
            }

            if (String.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentNullException(nameof(message), "Argument must not be null or white space.");
            }

            if (username.Contains(":"))
            {
                throw new ArgumentException("Argument must not include the ':' character.", nameof(username));
            }

            Username = username;
            Message = message;
        }

        #endregion

        #region Convenience Methods

        public static M_ChatMessage ParseSimpleText(string simpleText)
        {
            if (String.IsNullOrWhiteSpace(simpleText))
            {
                throw new ArgumentNullException(nameof(simpleText), "Argument can not be null or an empty string.");
            }

            var stringArray = simpleText.Split(':');

            if (!simpleText.Contains(": ") || stringArray.Length <= 1 || simpleText.Length <= 3 || stringArray[1].Length == 0)
            {
                throw new ArgumentException("Argument must be a correctly formatted message as simple text. " +
                                            "(i.e. test must look like \"[USERNAME]: [MESSAGE]\")", nameof(simpleText));
            }

            var username = stringArray[0];

            var message = stringArray[1].Substring(1);

            // Just in case message contains any :'s
            for (var i = 2; i < stringArray.Length; i++)
            {
                message += ":";
                message += stringArray[i];
            }

            return new M_ChatMessage(username, message);
        }

        public static String ToSimpleText(M_ChatMessage message)
        {
            return message.Username + ": " + message.Message;
        }

        #endregion

    }
}
