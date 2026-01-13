using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace iTunesSyncer.Extensions
{
    public class OverrideStream : Stream
    {
        private readonly Stream _baseStaream;

        public OverrideStream(Stream stream)
        {
            _baseStaream = stream;
        }

        /***************************************************************************************
         * Override Stream Class
         ***************************************************************************************/

        public override bool CanRead => _baseStaream.CanRead;

        public override bool CanSeek => _baseStaream.CanSeek;

        public override bool CanTimeout => _baseStaream.CanTimeout;

        public override bool CanWrite => _baseStaream.CanWrite;

        public override long Length => _baseStaream.Length;

        public override long Position { get => _baseStaream.Position; set => _baseStaream.Position = value; }

        public override int ReadTimeout { get => _baseStaream.ReadTimeout; set => _baseStaream.ReadTimeout = value; }

        public override int WriteTimeout { get => _baseStaream.WriteTimeout; set => _baseStaream.WriteTimeout = value; }

        public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback? callback, object? state)
            => _baseStaream.BeginRead(buffer, offset, count, callback, state);

        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback? callback, object? state)
            => _baseStaream.BeginWrite(buffer, offset, count, callback, state);

        public override void Close()
            => _baseStaream.Close();

        public override void CopyTo(Stream destination, int bufferSize)
            => _baseStaream.CopyTo(destination, bufferSize);

        public override Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken)
            => _baseStaream.CopyToAsync(destination, bufferSize, cancellationToken);

        public override ValueTask DisposeAsync()
            => _baseStaream.DisposeAsync();

        public override int EndRead(IAsyncResult asyncResult)
            => _baseStaream.EndRead(asyncResult);

        public override void EndWrite(IAsyncResult asyncResult)
            => _baseStaream.EndWrite(asyncResult);

        public override void Flush()
            => _baseStaream.Flush();

        public override Task FlushAsync(CancellationToken cancellationToken)
            => _baseStaream.FlushAsync(cancellationToken);

        public override int Read(byte[] buffer, int offset, int count)
            => _baseStaream.Read(buffer, offset, count);

        public override int Read(Span<byte> buffer)
            => _baseStaream.Read(buffer);

        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
            => _baseStaream.ReadAsync(buffer, offset, count, cancellationToken);

        public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
            => _baseStaream.ReadAsync(buffer, cancellationToken);

        public override int ReadByte()
            => _baseStaream.ReadByte();

        public override long Seek(long offset, SeekOrigin origin)
            => _baseStaream.Seek(offset, origin);

        public override void SetLength(long value)
            => _baseStaream.SetLength(value);

        public override void Write(byte[] buffer, int offset, int count)
            => _baseStaream.Write(buffer, offset, count);

        public override void Write(ReadOnlySpan<byte> buffer)
            => _baseStaream.Write(buffer);

        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
            => _baseStaream.WriteAsync(buffer, offset, count, cancellationToken);

        public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
            => _baseStaream.WriteAsync(buffer, cancellationToken);

        public override void WriteByte(byte value)
            => _baseStaream.WriteByte(value);
    }
}
