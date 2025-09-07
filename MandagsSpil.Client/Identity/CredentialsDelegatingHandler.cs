using System;
using Microsoft.AspNetCore.Components.WebAssembly.Http;

namespace MandagsSpil.Client.Identity;

public class CredentialsDelegatingHandler : DelegatingHandler
{
    public CredentialsDelegatingHandler()
    {
        InnerHandler = new HttpClientHandler();
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        request = request.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);
        return await base.SendAsync(request, cancellationToken);
    }
}
