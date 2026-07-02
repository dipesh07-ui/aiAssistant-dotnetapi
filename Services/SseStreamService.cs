using aiAssistant.api.Models;
using System.Collections.Concurrent;
using System.Threading.Channels;

namespace aiAssistant.api.Services
{
    public class SseStreamService
    {
        private readonly ConcurrentDictionary<Guid, Channel<ProgressEvent>>
        _streams = new();

        public void Create(Guid meetingId) =>
            _streams.TryAdd(meetingId, Channel.CreateUnbounded<ProgressEvent>());

        public async Task PushAsync(Guid meetingId, ProgressEvent evt)
        {
            if (_streams.TryGetValue(meetingId, out var ch))
                await ch.Writer.WriteAsync(evt);
        }

        public IAsyncEnumerable<ProgressEvent> Subscribe(
            Guid meetingId, CancellationToken ct)
        {
            var ch = _streams.GetOrAdd(
                meetingId, _ => Channel.CreateUnbounded<ProgressEvent>());
            return ch.Reader.ReadAllAsync(ct);
        }

        public void Close(Guid meetingId)
        {
            if (_streams.TryRemove(meetingId, out var ch))
                ch.Writer.Complete();
        }
    }
}
