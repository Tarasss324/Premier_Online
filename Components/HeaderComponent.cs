using Microsoft.Playwright;

namespace TestProject.Components;

public sealed class HeaderComponent
{
    public HeaderComponent(IPage page)
    {
        Root = page.Locator("header[uk-sticky]");
    }

    internal ILocator Root { get; }
    internal ILocator Logo => Root.Locator("a.uk-logo");
    internal ILocator LogoImage => Logo.GetByRole(AriaRole.Img);
    internal ILocator SearchInput => Root.Locator("#search");
    internal ILocator MenuButton => Root.Locator("button:has(i.fa-bars)");
    internal ILocator MobileMenu => Root.Locator(".blue-dropdown");
    internal ILocator MobileMenuNavigation => MobileMenu.Locator("ul.mobile-nav");
    internal ILocator MobileMenuEventsLink => MobileMenuNavigation.GetByRole(
        AriaRole.Link,
        new() { Name = "Events", Exact = true });
    internal ILocator HeaderNavigation => Root.Locator(".uk-navbar-left > ul.uk-navbar-nav");
    internal ILocator SignInLink => Root.GetByRole(
        AriaRole.Link,
        new() { Name = "Sign in", Exact = true });
    private ILocator CreateAccountLink => Root.GetByRole(
        AriaRole.Link,
        new() { Name = "Create Account", Exact = true });
    internal ILocator SignOutLink => Root.GetByRole(
        AriaRole.Link,
        new() { Name = "Sign out", Exact = true });
    internal ILocator ProfileLink => Root.Locator("a[href$='/action/personal_details']:visible");

    internal ILocator HeaderLink(string name) =>
        HeaderNavigation.GetByRole(
            AriaRole.Link,
            new() { Name = name, Exact = true });

    public Task OpenMobileMenuAsync() => MenuButton.ClickAsync();

    public Task ClickSignInAsync() => SignInLink.ClickAsync();

    public Task ClickCreateAccountAsync() => CreateAccountLink.ClickAsync();

    public Task ClickHeaderLinkAsync(string name) => HeaderLink(name).ClickAsync();
}
