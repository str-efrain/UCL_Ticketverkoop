using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Concurrent;
using System.Linq;

namespace UCL_Ticketverkoop.Hubs
{
    public class SeatHub : Hub
    {
        private static readonly ConcurrentDictionary<string, ConcurrentDictionary<string, string>> SeatSelections = new(StringComparer.OrdinalIgnoreCase);

        public async Task JoinGroup(int matchId, int vakId)
        {
            string groupName = GetGroupName(matchId, vakId);
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

            var seats = SeatSelections.GetOrAdd(groupName, _ => new ConcurrentDictionary<string, string>(StringComparer.OrdinalIgnoreCase));
            var reservedSeats = seats.Where(kvp => kvp.Value != Context.ConnectionId)
                .Select(kvp => kvp.Key)
                .ToList();

            if (reservedSeats.Count > 0)
            {
                await Clients.Caller.SendAsync("SeatsReserved", reservedSeats);
            }
        }

        public async Task<bool> TrySelectSeat(int matchId, int vakId, string seatId)
        {
            string groupName = GetGroupName(matchId, vakId);
            var seats = SeatSelections.GetOrAdd(groupName, _ => new ConcurrentDictionary<string, string>(StringComparer.OrdinalIgnoreCase));

            if (seats.TryGetValue(seatId, out string? owner) && owner != Context.ConnectionId)
            {
                return false;
            }

            seats[seatId] = Context.ConnectionId;
            await Clients.OthersInGroup(groupName).SendAsync("SeatReserved", seatId);
            return true;
        }

        public async Task UnselectSeat(int matchId, int vakId, string seatId)
        {
            string groupName = GetGroupName(matchId, vakId);
            if (!SeatSelections.TryGetValue(groupName, out var seats))
            {
                return;
            }

            if (seats.TryGetValue(seatId, out string? owner) && owner == Context.ConnectionId)
            {
                seats.TryRemove(seatId, out _);
                await Clients.OthersInGroup(groupName).SendAsync("SeatReleased", seatId);
            }
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            foreach (var group in SeatSelections)
            {
                var releasedSeats = group.Value
                    .Where(kvp => kvp.Value == Context.ConnectionId)
                    .Select(kvp => kvp.Key)
                    .ToList();

                if (releasedSeats.Count == 0)
                {
                    continue;
                }

                foreach (string seatId in releasedSeats)
                {
                    group.Value.TryRemove(seatId, out _);
                }

                await Clients.Group(group.Key).SendAsync("SeatsReleased", releasedSeats);
            }

            await base.OnDisconnectedAsync(exception);
        }

        private static string GetGroupName(int matchId, int vakId)
        {
            return $"match-{matchId}-vak-{vakId}";
        }
    }
}
