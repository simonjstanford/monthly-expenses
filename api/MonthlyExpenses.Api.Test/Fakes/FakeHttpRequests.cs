﻿using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using MonthlyExpenses.Api.Authentication;
using Moq;
using System.Text;
using System.Text.Json;

namespace MonthlyExpenses.Api.Test.Fakes;

public static class FakeHttpRequests
{
    internal static Mock<HttpRequest> CreateMockRequest(IHeaderDictionary header)
    {
        var mockRequest = new Mock<HttpRequest>();
        SetupCustomRequestHeader(mockRequest, header);
        return mockRequest;
    }

    internal static Mock<HttpRequest> CreateRequestWithoutHeader()
    {
        var mockRequest = new Mock<HttpRequest>();
        var header = new Mock<IHeaderDictionary>();
        SetupCustomRequestHeader(mockRequest, header.Object);
        return mockRequest;
    }

    internal static Mock<HttpRequest> CreateRequestWithoutAuthenticatedClaim()
    {
        var claim = FakeClaimsPrincipal.CreateClientPrincipalBase64(new List<string> { "anonymous" });
        return CreateMockRequestWithClaim(claim);
    }

    internal static Mock<HttpRequest> CreateRequestWithRandomClaim()
    {
        var claim = FakeClaimsPrincipal.CreateClientPrincipalBase64(new List<string> { "anonymous", "unknown" });
        return CreateMockRequestWithClaim(claim);
    }

    internal static Mock<HttpRequest> CreateRequestWithAuthenticatedClaim()
    {
        var claim = FakeClaimsPrincipal.CreateClientPrincipalBase64(new List<string> { "anonymous", "authenticated" });
        return CreateMockRequestWithClaim(claim);
    }

    internal static Mock<HttpRequest> CreateRequestWithUnAuthenticatedClaim()
    {
        var claim = FakeClaimsPrincipal.CreateClientPrincipalBase64(new List<string> { "anonymous", "authenticated" }, identityProvider: string.Empty);
        return CreateMockRequestWithClaim(claim);
    }

    internal static Mock<HttpRequest> CreateMockRequestWithBody(object? body, string contentType = "application/json")
    {
        var header = new Mock<IHeaderDictionary>();
        var mockRequest = CreateMockRequest(header.Object);

        if (body != null)
        {
            SetupRequestBody(contentType, body, mockRequest);
        }

        return mockRequest;
    }

    private static Mock<HttpRequest> CreateMockRequestWithClaim(string claim)
    {
        var header = new Mock<IHeaderDictionary>();
        var strings = new StringValues(claim);
        header.Setup(x => x.TryGetValue(OAuthAuthenticator.PrincipalHeader, out strings)).Returns(true);
        return CreateMockRequest(header.Object);
    }

    private static void SetupRequestBody(string contentType, object? body, Mock<HttpRequest> mockRequest)
    {
        var json = JsonSerializer.Serialize(body);
        var byteArray = Encoding.ASCII.GetBytes(json);
        var httpRequestStream = new MemoryStream(byteArray);
        httpRequestStream.Flush();
        httpRequestStream.Position = 0;
        mockRequest.Setup(x => x.Body).Returns(httpRequestStream);
        mockRequest.Setup(x => x.ContentType).Returns(contentType);
    }

    private static void SetupCustomRequestHeader(Mock<HttpRequest> mockRequest, IHeaderDictionary header)
    {
        mockRequest.Setup(x => x.Headers).Returns(header);
    }
}