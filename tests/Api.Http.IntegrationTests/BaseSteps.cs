﻿namespace Api.Http.IntegrationTests
{
    using Microsoft.AspNetCore.Mvc.Testing;
    using System.Net.Http;

    /// <summary>
    /// Defines the <see cref="BaseSteps" />
    /// </summary>
    public class BaseSteps
    {
        /// <summary>
        /// Gets the Client
        /// </summary>
        public HttpClient Client { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseSteps"/> class.
        /// </summary>
        public BaseSteps()
        {
            var factory = new WebApplicationFactory<Startup>();
            Client = factory.CreateClient();
        }
    }
}
