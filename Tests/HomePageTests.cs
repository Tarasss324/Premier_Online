using NUnit.Framework;
using TestProject.Config;

namespace TestProject.Tests;

[TestFixture]
public class HomePageTests : UiTestBase
{
    [Test]
    public async Task HomePageOpensSuccessfully()
    {
        await Expect(Page).ToHaveURLAsync($"{TestConfig.BaseUrl}/");
        await Expect(Page).ToHaveTitleAsync(
            "Premier Online - leading provider of online event registration for sports events");
    }

    [Test]
    public async Task ArabicLanguageIsApplied()
    {
        await Header.ClickHeaderLinkAsync("عربى");
        await Expect(Home.Html).ToHaveAttributeAsync("dir", "rtl");
        await Expect(Header.SearchInput).ToHaveAttributeAsync("placeholder", "البحث عن طريق اسم الحدث أو المنظم");
        await Expect(Header.HeaderLink("الفعاليات")).ToBeVisibleAsync();
        await Expect(Header.HeaderLink("مساعدة")).ToBeVisibleAsync();
        await Expect(Header.HeaderLink("English")).ToBeVisibleAsync();
    }
}
