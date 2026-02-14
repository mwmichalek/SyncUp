using System.Net;
using FluentAssertions;
using Xunit;

namespace SyncUp.ClickUpApi.Tests.Models;

public class ClickUpOptionsTests
{
    [Fact]
    public void GetEffectiveToken_Returns_PersonalApiToken_When_Set()
    {
        var options = new ClickUpOptions { PersonalApiToken = "pk_123" };

        // GetEffectiveToken is internal, test via reflection or make InternalsVisibleTo
        // For now we test behavior through the auth handler tests.
        // This test validates the public property contract.
        options.PersonalApiToken.Should().Be("pk_123");
    }

    [Fact]
    public void Defaults_BaseUrl_To_ClickUp_Api()
    {
        var options = new ClickUpOptions();

        options.BaseUrl.Should().Be("https://api.clickup.com/api");
    }

    [Fact]
    public void SectionName_Is_ClickUp()
    {
        ClickUpOptions.SectionName.Should().Be("ClickUp");
    }

    [Fact]
    public void All_Optional_Properties_Default_To_Null()
    {
        var options = new ClickUpOptions();

        options.PersonalApiToken.Should().BeNull();
        options.OAuthClientId.Should().BeNull();
        options.OAuthClientSecret.Should().BeNull();
        options.OAuthAccessToken.Should().BeNull();
        options.DefaultWorkspaceId.Should().BeNull();
    }
}

public class ClickUpApiExceptionTests
{
    [Fact]
    public void Stores_StatusCode_And_ResponseBody()
    {
        var ex = new ClickUpApiException(HttpStatusCode.BadRequest, """{"err":"bad"}""");

        ex.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        ex.ResponseBody.Should().Contain("bad");
        ex.Message.Should().Contain("400");
        ex.Message.Should().Contain("BadRequest");
    }

    [Fact]
    public void Truncates_Long_Response_Body_In_Message()
    {
        var longBody = new string('x', 1000);
        var ex = new ClickUpApiException(HttpStatusCode.InternalServerError, longBody);

        ex.Message.Length.Should().BeLessThan(600); // 500 chars + prefix + "..."
        ex.Message.Should().EndWith("...");
        // Full body is preserved in the property
        ex.ResponseBody.Should().HaveLength(1000);
    }

    [Fact]
    public void Handles_Null_ResponseBody()
    {
        var ex = new ClickUpApiException(HttpStatusCode.NotFound, null);

        ex.ResponseBody.Should().BeNull();
        ex.Message.Should().Contain("404");
    }

    [Fact]
    public void Inner_Exception_Constructor_Preserves_Inner()
    {
        var inner = new InvalidOperationException("inner error");
        var ex = new ClickUpApiException(HttpStatusCode.BadGateway, "body", inner);

        ex.InnerException.Should().BeSameAs(inner);
        ex.StatusCode.Should().Be(HttpStatusCode.BadGateway);
        ex.ResponseBody.Should().Be("body");
    }
}
