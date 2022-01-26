using System.IO;
using System.Text;

using Akka.Actor;

using static WinTail.TailActorMessages;

namespace WinTail
{
    /// <summary>
    /// 
    /// </summary>
    public class TailActor : UntypedActor
    {
        private string _filePath;
        private IActorRef _reporterActor;
        private readonly FileObserver _observer;
        private readonly Stream _fileStream;
        private readonly StreamReader _fileStreamReader;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="reporterActor">Should be the ConsoleWriterActor</param>
        public TailActor(string filePath, IActorRef reporterActor)
        {
            _reporterActor = reporterActor;
            _filePath = filePath;

            // start watching file for changes
            _observer = new FileObserver(
                Self, Path.GetFullPath(_filePath));
            _observer.Start();

            // open the file stream with shared read/write permissions
            // (so file can be written to while open)
            _fileStream = new FileStream(
                Path.GetFullPath(_filePath),
                FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

            _fileStreamReader = new StreamReader(_fileStream, Encoding.UTF8);

            // read the initial contents of the file and send it to console as first msg
            var text = _fileStreamReader.ReadToEnd();
            Self.Tell(new InitialRead(_filePath, text));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        protected override void OnReceive(object message)
        {
            if (message is FileWrite) {
                // move file cursor forward
                // pull results from cursor to end of file and write to output
                // (this is assuming a log file type format that is append-only)
                var text = _fileStreamReader.ReadToEnd();

                if (!string.IsNullOrWhiteSpace(text)) {
                    _reporterActor.Tell(text);
                }

            } else if (message is FileError) {
                var fe = message as FileError;
                _reporterActor.Tell($"Tail error: {fe.Reason}");

            } else if (message is InitialRead) {
                var ir = message as InitialRead;
                _reporterActor.Tell(ir.Text);
            }
        }
    }
}
