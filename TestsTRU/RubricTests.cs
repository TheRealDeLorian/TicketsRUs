using Bunit;
using FluentAssertions;
using FluentAssertions.Execution;
using LibraryTRU.Data;
using LibraryTRU.Data.DTOs;
using LibraryTRU.Exceptions;
using LibraryTRU.IServices;
using MauiTRU.Database;
using MauiTRU.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Engine.ClientProtocol;
using System.Net.Http;
using System.Net.Http.Json;



namespace TestsTRU;


public class RubricTests : IClassFixture<TRUWebAppFactory>
{
    HttpClient client;
    public RubricTests(TRUWebAppFactory factory)
    {
        client = factory.CreateDefaultClient();
    }

    [Fact]
    public async void SuccessfulScanUpdatesDatabase()
    {
        //Arrange: create fake maui app and new ticket
        using var mauiApp = new TestContext();
        mauiApp.Services.AddSingleton<IDbPath, TestDbPath>();
        mauiApp.Services.AddScoped<ITicketService, MauiTicketService>();
        mauiApp.Services.AddScoped<IConcertService, MauiConcertService>();
        mauiApp.Services.AddSingleton<LocalTRUDatabase>();
        mauiApp.Services.AddSingleton(client);
        var db = mauiApp.Services.GetService<LocalTRUDatabase>();
        var result = await client.PostAsJsonAsync("api/ticket/new", new TicketDTO() { Email = "test@example.com", ConcertId = 1 });
        var ticket = result.Content.ReadFromJsonAsync<Ticket>().Result;
        
        await db.UpdateLocalDbFromMainDb();

        //Act
        await db.ScanTicketAsync(ticket.Qrhash);

        var alltickets = await db.GetTicketsAsync();

        var testticket = alltickets.Where(t => t.Qrhash == ticket.Qrhash).FirstOrDefault();

        testticket.Timescanned.Should().NotBeNull();
    }

    [Fact]
    public async void FailedScanDoesntUpdateDatabase()
    {
        //Arrange: create fake maui app and new ticket
        using var mauiApp = new TestContext();
        mauiApp.Services.AddSingleton<IDbPath, TestDbPath>();
        mauiApp.Services.AddScoped<ITicketService, MauiTicketService>();
        mauiApp.Services.AddScoped<IConcertService, MauiConcertService>();
        mauiApp.Services.AddSingleton<LocalTRUDatabase>();
        mauiApp.Services.AddSingleton(client);
        var db = mauiApp.Services.GetService<LocalTRUDatabase>();
        var result = await client.PostAsJsonAsync("api/ticket/new", new TicketDTO() { Email = "test@example.com", ConcertId = 1 });
        var ticket = result.Content.ReadFromJsonAsync<Ticket>().Result;

        await db.UpdateLocalDbFromMainDb();

        //Act
        try
        {
            await db.ScanTicketAsync("666");
        }
        catch (TicketNotFoundException)
        {
            Console.WriteLine(typeof(TicketNotFoundException) + " was thrown successfully.");
            Assert.True(true);
            return;
        }

        Assert.Fail();
    }

    [Fact]
    public async void APIAddressChangeRefreshesLocalDb()
    {
        //Arrange: create fake maui app and new ticket
        using var mauiApp = new TestContext();
        mauiApp.Services.AddSingleton<IDbPath, TestDbPath>();
        mauiApp.Services.AddScoped<ITicketService, MauiTicketService>();
        mauiApp.Services.AddScoped<IConcertService, MauiConcertService>();
        mauiApp.Services.AddSingleton<LocalTRUDatabase>();
        mauiApp.Services.AddSingleton(client);
        var db = mauiApp.Services.GetService<LocalTRUDatabase>();

        //Act part 1
        await db.UpdateLocalDbFromMainDb();
        var ticketlist = await db.GetTicketsAsync();
        
        //Assert part 1
        ticketlist.Should().NotBeEmpty();
        
        //Act part 2
        await db.DeleteDatabase();
        ticketlist = await db.GetTicketsAsync();

        //Assert part 2
        ticketlist.Should().BeEmpty();


    }




}