using System.Text.RegularExpressions;
using Ganss.Xss;
using Markdig;

namespace Blog.Infrastructure.Markdown;
public interface IMarkdownRenderer
{
    string ToHtml(string markdown);
}

public class MarkdownRenderer : IMarkdownRenderer
{
    private readonly MarkdownPipeline _pipeline;
    private readonly HtmlSanitizer _sanitizer;

    
    private static readonly Regex MentionRegex =
        new(@"(?<!\w)@([A-Za-z0-9_]+)", RegexOptions.Compiled);

    
    private static readonly Regex HashtagRegex =
        new(@"(?<!\w)#([A-Za-z0-9_]+)", RegexOptions.Compiled);

    
    private static readonly Regex PostLinkRegex =
        new(@"(?<!\w)/post/(\d+)", RegexOptions.Compiled);

    public MarkdownRenderer()
    {
        _pipeline = new MarkdownPipelineBuilder()
            .UseAdvancedExtensions()
            .UseSoftlineBreakAsHardlineBreak()
            .UseAutoLinks()
            .UseEmphasisExtras()
            .UsePipeTables()
            .UseListExtras()
            .UseGenericAttributes()
            .Build();

        _sanitizer = new HtmlSanitizer();
        
        _sanitizer.AllowedTags.Add("a");
        _sanitizer.AllowedTags.Add("p");
        _sanitizer.AllowedTags.Add("br");
        _sanitizer.AllowedTags.Add("strong");
        _sanitizer.AllowedTags.Add("em");
        _sanitizer.AllowedTags.Add("del");
        _sanitizer.AllowedTags.Add("blockquote");
        _sanitizer.AllowedTags.Add("pre");
        _sanitizer.AllowedTags.Add("code");
        _sanitizer.AllowedTags.Add("ul");
        _sanitizer.AllowedTags.Add("ol");
        _sanitizer.AllowedTags.Add("li");
        _sanitizer.AllowedTags.Add("h1");
        _sanitizer.AllowedTags.Add("h2");
        _sanitizer.AllowedTags.Add("h3");
        
        _sanitizer.AllowedAttributes.Add("href");
        _sanitizer.AllowedAttributes.Add("class");
        _sanitizer.AllowedAttributes.Add("rel");
        _sanitizer.AllowedAttributes.Add("target");
    }

    public string ToHtml(string markdown)
    {
        if (string.IsNullOrWhiteSpace(markdown))
            return string.Empty;
        
        markdown = ProcessSmartElements(markdown);
        
        var html = Markdig.Markdown.ToHtml(markdown, _pipeline);
        
        html = _sanitizer.Sanitize(html);

        return html;
    }

    private static string ProcessSmartElements(string text)
    {

        text = MentionRegex.Replace(text, m =>
        {
            var user = m.Groups[1].Value;
            return $"[@{user}](/Profile/User?name={user})";
        });
        
        text = HashtagRegex.Replace(text, m =>
        {
            var tag = m.Groups[1].Value;
            return $"[#{tag}](/tag/{tag})";
        });


        text = PostLinkRegex.Replace(text, m =>
        {
            var id = m.Groups[1].Value;
            return $"[/post/{id}](/Post/{id})";
        });

        return text;
    }
}
