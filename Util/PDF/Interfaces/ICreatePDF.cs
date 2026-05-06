using Domains.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Util.PDF.Interfaces
{
    public interface ICreatePDF
    {
        byte[] CreateTicketPDFDocument(List<Ticket> tickets);
        byte[] CreateAbonnementPDFDocument(List<Abonnement> abonnements);
    }
}
