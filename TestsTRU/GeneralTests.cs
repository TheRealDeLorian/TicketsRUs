using FluentAssertions;
using LibraryTRU.Data;
using LibraryTRU.Data.DTOs;
using System.Net.Http.Json;

namespace TestsTRU;

public class GeneralTests : IClassFixture<TRUWebAppFactory>
{
    HttpClient client;
    public GeneralTests(TRUWebAppFactory factory)
    {
        client = factory.CreateDefaultClient();
    }

    [Fact]
    public async void GetAllTicketsTest()
    {
        var tickets = await client.GetFromJsonAsync<IEnumerable<Ticket>>("api/ticket/getall");
        tickets.Where(o => o.Id == 1).Should().HaveCount(1);
    }

    [Fact]
    public async void GetOneTicketById()
    {
        var ticket = await client.GetFromJsonAsync<Ticket>("api/ticket/1");
        ticket.Id.Should().Be(1);
    }

    [Fact]
    public async void PostTicketToDbWithDTOReturnsCreatedTicket()
    {
        string testEmail = "test@example.com";
        TicketDTO data = new() { Email = testEmail, ConcertId = 1 };
        var ticket = await client.PostAsJsonAsync("/api/ticket/new", data);
        ticket.Content.ReadFromJsonAsync<Ticket>().Result.Email.Should().Be(testEmail);
    }

    //[Fact]
    //public async void EmailServiceCanSend()
    //{
    //    EmailInfoDTO emailDTO = new()
    //    {
    //        Email = "thenewdorian21@gmail.com",
    //        Subject = "Test from test explorer",
    //        Message = "Test"
    //    };

    //    HttpResponseMessage response = await client.PostAsJsonAsync("/api/email", emailDTO);
    //    response.IsSuccessStatusCode.Should().BeTrue();
    //}
}
