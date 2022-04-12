using AHKUpdater.Library.Enums;
using System;
using System.Timers;

namespace AHKUpdater.Model
{
    public class Message
    {
        internal static Action<Message> OnTimeElapsed;
        private Timer _messageValidTimer;
        private MessageType _messageType;
        private Guid _messageId = Guid.NewGuid();

        public Message () { }

        public Message ( string msg )
        {
            Msg = msg;
            Type = 0;
        }

        public Message ( int type, string msg )
        {
            Msg = msg;
            Type = (MessageType) type;
        }

        public Message ( MessageType type, string msg )
        {
            Msg = msg;
            Type = type;
        }

        public string Msg { get; set; }
        public MessageType Type
        {
            get { return _messageType; }
            set
            {
                if ( (int) value >= 2 )
                {
                    _messageValidTimer = new Timer
                    {
                        Interval = (int) value * 1000,
                        Enabled = true,
                        AutoReset = false
                    };
                    _messageValidTimer.Elapsed += MessageValidTimer_Elapsed;
                    _messageValidTimer.Start();
                }
                _messageType = value;
            }
        }

        private void MessageValidTimer_Elapsed ( object sender, ElapsedEventArgs e ) { OnTimeElapsed.Invoke( this ); }
    }
}
