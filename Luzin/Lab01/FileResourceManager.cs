using System;
using System.IO;


namespace Lab1
{
    public class FileResourceManager : IDisposable
    {
        private FileStream _fileStream;
        private StreamWriter _writer;
        private StreamReader _reader;
        private bool _disposed = false;
        private string _filePath;
        private FileMode _fileMode;

        public FileResourceManager(string filePath, FileMode mode)
        {
            _filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
            _fileMode = mode;
        }

        public void OpenForWriting()
        {
            if (_disposed) throw new ObjectDisposedException(nameof(FileResourceManager));

            _fileStream = new FileStream(_filePath, _fileMode, FileAccess.Write);
            _writer = new StreamWriter(_fileStream);
        }

        public void OpenForReading()
        {
            if (_disposed) throw new ObjectDisposedException(nameof(FileResourceManager));

            _fileStream = new FileStream(_filePath, _fileMode, FileAccess.Read);
            _reader = new StreamReader(_fileStream);
        }

        public void WriteLine(string text)
        {
            if (_disposed) throw new ObjectDisposedException(nameof(FileResourceManager));
            if (_writer == null) throw new InvalidOperationException("File not opened for writing");

            _writer.WriteLine(text);
            _writer.Flush();
        }

        public string ReadAllText()
        {
            if (_disposed) throw new ObjectDisposedException(nameof(FileResourceManager));
            if (_reader == null) throw new InvalidOperationException("File not opened for reading");

            return _reader.ReadToEnd();
        }

        public void AppendText(string text)
        {
            if (_disposed) throw new ObjectDisposedException(nameof(FileResourceManager));

            using (var writer = File.AppendText(_filePath))
            {
                writer.Write(text);
            }
        }

        public FileInfo GetFileInfo()
        {
            if (_disposed) throw new ObjectDisposedException(nameof(FileResourceManager));

            if (!File.Exists(_filePath))
                throw new FileNotFoundException($"File not found: {_filePath}", _filePath);

            return new FileInfo(_filePath);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _writer?.Dispose();
                    _reader?.Dispose();
                    _fileStream?.Dispose();
                }

                _writer = null;
                _reader = null;
                _fileStream = null;
                _disposed = true;
            }
        }

        ~FileResourceManager()
        {
            Dispose(false);
        }
    }
}