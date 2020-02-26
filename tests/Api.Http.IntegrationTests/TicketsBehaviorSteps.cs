﻿namespace Api.Http.IntegrationTests
{
    using Domain;
    using FluentAssertions;
    using Newtonsoft.Json;
    using System;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Mime;
    using System.Text;
    using System.Threading.Tasks;
    using TechTalk.SpecFlow;

    /// <summary>
    /// Defines the <see cref="TicketsBehaviorSteps" />
    /// </summary>
    [Binding]
    public class TicketsBehaviorSteps : BaseSteps
    {
        private const string CreateTicket = "/api/Tickets/create";
        private const string ReadTicket = "/api/Tickets/read";
        private const string AddConversation = "/api/Tickets/add-conversation";
        private const string AddNote = "/api/Tickets/add-note";
        private const string UpdateStatus = "/api/Tickets/update-status";
        private const string UpdateNote = "/api/Tickets/update-note";

        private HttpResponseMessage _response;
        private Conversation _conversation;
        private Note _note;
        private string _id;
        private string _noteId;
        private Ticket _ticket;

        /// <summary>
        /// Given A ticket id
        /// </summary>
        [Given(@"A ticket id")]
        public void GivenATicketId()
        {
            _id = Guid.NewGuid().ToString();
        }

        /// <summary>
        /// When Update Note Status
        /// </summary>
        [When(@"Update Note Status")]
        public async Task WhenUpdateNoteStatus()
        {
            _response = await Client.PutAsync($"{UpdateNote}?id={_id}&noteId={_noteId}", null);
        }

        /// <summary>
        /// Then Note Status should be Open
        /// </summary>
        [Then(@"Note Status should be Open")]
        public async Task ThenNoteStatusShouldBeFalse()
        {
            _response.StatusCode.Should().Be(HttpStatusCode.OK);
            _ticket = JsonConvert.DeserializeObject<Ticket>(await _response.Content.ReadAsStringAsync());
            _ticket.Should().NotBeNull();

            var note = _ticket.Notes.LastOrDefault();
            note.Should().NotBeNull();
            note?.Closed.Should().Be(false);
        }

        /// <summary>
        /// Then Note Status should be Closed
        /// </summary>
        [Then(@"Note Status should be Closed")]
        public async Task ThenNoteStatusShouldBeTrue()
        {
            _response.StatusCode.Should().Be(HttpStatusCode.OK);
            _ticket = JsonConvert.DeserializeObject<Ticket>(await _response.Content.ReadAsStringAsync());
            _ticket.Should().NotBeNull();

            var note = _ticket.Notes.LastOrDefault();
            note.Should().NotBeNull();
            note?.Closed.Should().Be(true);
        }

        /// <summary>
        /// When Ticket status is set to [status]
        /// </summary>
        /// <param name="status">Ticket status</param>
        [When(@"Ticket status is set to (.*)")]
        public async Task WhenTicketStatusIsSetTo(int status)
        {
            _response = await Client.PutAsync($"{UpdateStatus}?id={_id}&status={status}", null);
        }

        /// <summary>
        /// Then Ticket status should be [status]
        /// </summary>
        /// <param name="status">Ticket status</param>
        [Then(@"Ticket status should be (.*)")]
        public async Task ThenTicketStatusShouldBe(int status)
        {
            _response.StatusCode.Should().Be(HttpStatusCode.OK);
            _ticket = JsonConvert.DeserializeObject<Ticket>(await _response.Content.ReadAsStringAsync());
            _ticket.Should().NotBeNull();

            _ticket.Status.Should().Be((TicketStatus)status);
        }

        /// <summary>
        /// Given A Note
        /// </summary>
        [Given(@"A Note")]
        public void GivenANote()
        {
            _noteId = Guid.NewGuid().ToString();
            _note = new Note("New note");
        }

        /// <summary>
        /// When A note is added
        /// </summary>
        [When(@"A note is added")]
        public async Task WhenANoteIsAdded()
        {
            var json = JsonConvert.SerializeObject(_note);
            var content = new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json);
            _response = await Client.PostAsync($"{AddNote}?id={_id}", content);
        }

        /// <summary>
        /// Then Note is available in the ticket
        /// </summary>
        /// <returns></returns>
        [Then(@"Note is available in the ticket")]
        public async Task ThenNoteIsAvailableInTheTicket()
        {
            _response.StatusCode.Should().Be(HttpStatusCode.OK);
            _ticket = JsonConvert.DeserializeObject<Ticket>(await _response.Content.ReadAsStringAsync());
            _ticket.Should().NotBeNull();

            var note = _ticket.Notes.LastOrDefault();
            note.Should().NotBeNull();
            note?.Content.Should().Be(_note.Content);
        }


        /// <summary>
        /// When A conversation is added
        /// </summary>
        /// <returns></returns>
        [When(@"A conversation is added")]
        public async Task WhenAConversationIsAdded()
        {
            var json = JsonConvert.SerializeObject(_conversation);
            var content = new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json);
            _response = await Client.PostAsync($"{AddConversation}?id={_id}", content);
        }

        /// <summary>
        /// Then Conversation is available in the ticket
        /// </summary>
        /// <returns></returns>
        [Then(@"Conversation is available in the ticket")]
        public async Task ThenItIsAvailableInTheTicket()
        {
            _response.StatusCode.Should().Be(HttpStatusCode.OK);
            _ticket = JsonConvert.DeserializeObject<Ticket>(await _response.Content.ReadAsStringAsync());
            _ticket.Should().NotBeNull();

            var conversation = _ticket.Conversations.LastOrDefault();
            conversation.Should().NotBeNull();
            conversation?.Title.Should().Be(_conversation.Title);
            conversation?.Content.Should().Be(_conversation.Content);
        }

        /// <summary>
        /// When Reading ticket by a valid id
        /// </summary>
        [When(@"Reading ticket by a valid id")]
        public async Task WhenReadingTicketByAValidId()
        {
            _response = await Client.GetAsync($"{ReadTicket}?id={_id}");
        }

        /// <summary>
        /// Then It should return a ticket
        /// </summary>
        /// <returns></returns>
        [Then(@"It should return a ticket")]
        public async Task ThenItShouldReturnATicket()
        {
            _response.StatusCode.Should().Be(HttpStatusCode.OK);
            _ticket = JsonConvert.DeserializeObject<Ticket>(await _response.Content.ReadAsStringAsync());
            _ticket.Should().NotBeNull();
        }

        /// <summary>
        /// Given A Conversation
        /// </summary>
        [Given(@"A Conversation")]
        public void GivenAConversation()
        {
            _conversation = new Conversation
            {
                Title = "Hello",
                Content = "World"
            };
        }

        /// <summary>
        /// When I create a new Ticket
        /// </summary>
        [When(@"I create a new Ticket")]
        public async Task WhenICreateANewTicket()
        {
            var json = JsonConvert.SerializeObject(_conversation);
            var content = new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json);
            _response = await Client.PostAsync(CreateTicket, content);
        }

        /// <summary>
        /// Then A Ticket should have been created
        /// </summary>
        [Then(@"A Ticket should have been created")]
        public async Task ThenATicketShouldHaveBeenCreated()
        {
            _response.StatusCode.Should().Be(HttpStatusCode.OK);
            _ticket = JsonConvert.DeserializeObject<Ticket>(await _response.Content.ReadAsStringAsync());
            _ticket.Should().NotBeNull();
        }

        /// <summary>
        /// Then Its status should be Unassigned
        /// </summary>
        [Then(@"Its status should be Unassigned")]
        public void ThenItsStatusShouldBeUnassigned()
        {
            _ticket.Status.Should().Be(TicketStatus.Unassigned);
        }

        /// <summary>
        /// Then It should have [count]conversation
        /// </summary>
        /// <param name="count">The count<see cref="int"/></param>
        [Then(@"It should have (.*) conversation")]
        public void ThenItShouldHaveConversation(int count)
        {
            _ticket.Conversations.Count.Should().Be(count);
        }

        /// <summary>
        /// Then It should have only [count]notes
        /// </summary>
        /// <param name="count">The count<see cref="int"/></param>
        [Then(@"It should have (.*) notes")]
        public void ThenItShouldHaveNotes(int count)
        {
            _ticket.Notes.Count.Should().Be(count);
        }

    }
}
