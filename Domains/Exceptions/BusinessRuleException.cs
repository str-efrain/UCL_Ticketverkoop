using Domains.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Domains.Exceptions
{
    public class BusinessRuleException : Exception
    {
        public BusinessRuleException(string message) : base(message) { }
    }
    public class TicketLimitExceededException : BusinessRuleException
    {
        public TicketLimitExceededException(int limit)
            : base($"Je mag maximaal {limit} tickets per wedstrijd kopen.") { }
    }

    public class TicketSalesNotActiveException : BusinessRuleException
    {
        public TicketSalesNotActiveException(DateTime startDate, DateTime endDate)
            : base($"De ticketverkoop is actief tussen {startDate.ToShortDateString} en {endDate.ToShortDateString}") { }
    }

    public class SubscriptionSalesNotActiveException : BusinessRuleException
    {
        public SubscriptionSalesNotActiveException(DateTime endDate)
            : base($"De abonnement verkoop is gestopt op {endDate.ToShortDateString}") { }
    }

    public class MaxTicketsPerMatchDayExeededException : BusinessRuleException
    {
        public MaxTicketsPerMatchDayExeededException(int aantal)
            : base($"Je kunt maximum naar {aantal} wedstrijd(en) per dag gaan.") { }
    }

    public class SeatIsTakenException : BusinessRuleException
    {
        public SeatIsTakenException(string seatNumber, string vakNaam, string stadiumNaam)
            : base($"zitje {seatNumber} in vak {vakNaam} in {stadiumNaam} is al bezet.") { }
    }

}
