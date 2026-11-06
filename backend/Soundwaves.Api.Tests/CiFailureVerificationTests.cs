
using Xunit;

namespace Soundwaves.Api.Tests;

public class CiFailureVerificationTests
{
    [Fact]
    public void SOC2_ControlledFailure_VerifiesCiDetection()
    {
        // Temporary negative test for SOC 2 AC-06.
        // This test intentionally fails to verify CI detection.
        Assert.Fail(
            "SOC 2 AC-06: Controlled CI failure verification.");
    }
}
