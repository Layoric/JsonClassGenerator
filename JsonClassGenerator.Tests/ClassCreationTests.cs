using NUnit.Framework;
using ServiceStack;

namespace JsonClassGenerator.Tests
{
    [TestFixture]
    public class ClassCreationTests
    {
        [Test]
        public void CreateSimpleClass()
        {
            var simpleObj = "{ \"hello\": \"world\" }";
            string cSharpString = JsonCSharpGenerator.FromJsonObject(simpleObj).Replace("\r\n", "\n");
            string expected =
                @"public class RootObject
{
    public string Hello { get;set; }
}".Replace("\r\n","\n");
            Assert.That(cSharpString, Is.Not.Null);
            Assert.That(cSharpString.Length, Is.GreaterThan(0));
            Assert.That(cSharpString.Trim(), Is.EqualTo(expected.Trim()));
        }

        public class Hello
        {
            public string Name { get; set; }
        }

        public class RootObject
        {
            public Hello Hello { get; set; }
        }

        [Test]
        public void CreateSimpleClass2()
        {
            var simpleObj = new RootObject() { Hello = new Hello { Name = "World" } }.ToJson();
            string cSharpString = JsonCSharpGenerator.FromJsonObject(simpleObj).Replace("\r\n", "\n");
            string expected =
                @"public class Hello
{
    public string Name { get;set; }
}

public class RootObject
{
    public Hello Hello { get;set; }
}".Replace("\r\n", "\n");
            Assert.That(cSharpString, Is.Not.Null);
            Assert.That(cSharpString.Length, Is.GreaterThan(0));
            Assert.That(cSharpString.Trim(), Is.EqualTo(expected.Trim()));
        }

        [Test]
        public void CreateSimpleClassFromGitHubExample()
        {
            string input =
                "{\r\n  \"state\": \"success\",\r\n  \"target_url\": \"https://example.com/build/status\",\r\n  \"description\": \"The build succeeded!\",\r\n  \"context\": \"continuous-integration/jenkins\"\r\n}";
            string cSharpString = JsonCSharpGenerator.FromJsonObject(input).Replace("\r\n", "\n");
            string expected = @"public class RootObject
{
    public string State { get;set; }
    public string TargetUrl { get;set; }
    public string Description { get;set; }
    public string Context { get;set; }
}".Replace("\r\n", "\n");
            Assert.That(cSharpString, Is.Not.Null);
            Assert.That(cSharpString.Length, Is.GreaterThan(0));
            Assert.That(cSharpString.Trim(), Is.EqualTo(expected.Trim()));
        }

        [Test]
        public void CreateComplexClassesFromGitHubExample()
        {
            string input = @"{
  ""state"": ""success"",
  ""sha"": ""6dcb09b5b57875f334f61aebed695e2e4193db5e"",
  ""total_count"": 2,
  ""float_test"": 1.2,
  ""statuses"": [
    {
      ""created_at"": ""2012-07-20T01:19:13Z"",
      ""updated_at"": ""2012-07-20T01:19:13Z"",
      ""state"": ""success"",
      ""target_url"": ""https://ci.example.com/1000/output"",
      ""description"": ""Build has completed successfully"",
      ""id"": 1,
      ""url"": ""https://api.github.com/repos/octocat/Hello-World/statuses/1"",
      ""context"": ""continuous-integration/jenkins""
    },
    {
      ""created_at"": ""2012-08-20T01:19:13Z"",
      ""updated_at"": ""2012-08-20T01:19:13Z"",
      ""state"": ""success"",
      ""target_url"": ""https://ci.example.com/2000/output"",
      ""description"": ""Testing has completed successfully"",
      ""id"": 2,
      ""url"": ""https://api.github.com/repos/octocat/Hello-World/statuses/2"",
      ""context"": ""security/brakeman""
    }
  ],
  ""repository"": {
    ""id"": 1296269,
    ""owner"": {
      ""login"": ""octocat"",
      ""id"": 1,
      ""avatar_url"": ""https://github.com/images/error/octocat_happy.gif"",
      ""gravatar_id"": """",
      ""url"": ""https://api.github.com/users/octocat"",
      ""html_url"": ""https://github.com/octocat"",
      ""followers_url"": ""https://api.github.com/users/octocat/followers"",
      ""following_url"": ""https://api.github.com/users/octocat/following{/other_user}"",
      ""gists_url"": ""https://api.github.com/users/octocat/gists{/gist_id}"",
      ""starred_url"": ""https://api.github.com/users/octocat/starred{/owner}{/repo}"",
      ""subscriptions_url"": ""https://api.github.com/users/octocat/subscriptions"",
      ""organizations_url"": ""https://api.github.com/users/octocat/orgs"",
      ""repos_url"": ""https://api.github.com/users/octocat/repos"",
      ""events_url"": ""https://api.github.com/users/octocat/events{/privacy}"",
      ""received_events_url"": ""https://api.github.com/users/octocat/received_events"",
      ""type"": ""User"",
      ""site_admin"": false
    },
    ""name"": ""Hello-World"",
    ""full_name"": ""octocat/Hello-World"",
    ""description"": ""This your first repo!"",
    ""private"": false,
    ""fork"": false,
    ""url"": ""https://api.github.com/repos/octocat/Hello-World"",
    ""html_url"": ""https://github.com/octocat/Hello-World""
  },
  ""commit_url"": ""https://api.github.com/repos/octocat/Hello-World/6dcb09b5b57875f334f61aebed695e2e4193db5e"",
  ""url"": ""https://api.github.com/repos/octocat/Hello-World/6dcb09b5b57875f334f61aebed695e2e4193db5e/status""
}";
            string cSharpString = JsonCSharpGenerator.FromJsonObject(input).Replace("\r\n", "\n");
            string expected = @"public class Statuses
{
    public DateTime CreatedAt { get;set; }
    public DateTime UpdatedAt { get;set; }
    public string State { get;set; }
    public string TargetUrl { get;set; }
    public string Description { get;set; }
    public int Id { get;set; }
    public string Url { get;set; }
    public string Context { get;set; }
}

public class Owner
{
    public string Login { get;set; }
    public int Id { get;set; }
    public string AvatarUrl { get;set; }
    public DateTime GravatarId { get;set; }
    public string Url { get;set; }
    public string HtmlUrl { get;set; }
    public string FollowersUrl { get;set; }
    public string FollowingUrl { get;set; }
    public string GistsUrl { get;set; }
    public string StarredUrl { get;set; }
    public string SubscriptionsUrl { get;set; }
    public string OrganizationsUrl { get;set; }
    public string ReposUrl { get;set; }
    public string EventsUrl { get;set; }
    public string ReceivedEventsUrl { get;set; }
    public string Type { get;set; }
    public bool SiteAdmin { get;set; }
}

public class Repository
{
    public int Id { get;set; }
    public Owner Owner { get;set; }
    public string Name { get;set; }
    public string FullName { get;set; }
    public string Description { get;set; }
    public bool Private { get;set; }
    public bool Fork { get;set; }
    public string Url { get;set; }
    public string HtmlUrl { get;set; }
}

public class RootObject
{
    public string State { get;set; }
    public string Sha { get;set; }
    public int TotalCount { get;set; }
    public float FloatTest { get;set; }
    public List<Statuses> Statuses { get;set; }
    public Repository Repository { get;set; }
    public string CommitUrl { get;set; }
    public string Url { get;set; }
}".Replace("\r\n", "\n");
            Assert.That(cSharpString, Is.Not.Null);
            Assert.That(cSharpString.Length, Is.GreaterThan(0));
            Assert.That(cSharpString.Trim(), Is.EqualTo(expected.Trim()));
        }

        [Test]
        public void ComplexGitHubArrayExample()
        {
            string input = @"[
  {
    ""url"": ""https://api.github.com/repos/octocat/Hello-World/releases/1"",
    ""html_url"": ""https://github.com/octocat/Hello-World/releases/v1.0.0"",
    ""assets_url"": ""https://api.github.com/repos/octocat/Hello-World/releases/1/assets"",
    ""upload_url"": ""https://uploads.github.com/repos/octocat/Hello-World/releases/1/assets{?name,label}"",
    ""tarball_url"": ""https://api.github.com/repos/octocat/Hello-World/tarball/v1.0.0"",
    ""zipball_url"": ""https://api.github.com/repos/octocat/Hello-World/zipball/v1.0.0"",
    ""id"": 1,
    ""tag_name"": ""v1.0.0"",
    ""target_commitish"": ""master"",
    ""name"": ""v1.0.0"",
    ""body"": ""Description of the release"",
    ""draft"": false,
    ""prerelease"": false,
    ""created_at"": ""2013-02-27T19:35:32Z"",
    ""published_at"": ""2013-02-27T19:35:32Z"",
    ""author"": {
      ""login"": ""octocat"",
      ""id"": 1,
      ""avatar_url"": ""https://github.com/images/error/octocat_happy.gif"",
      ""gravatar_id"": """",
      ""url"": ""https://api.github.com/users/octocat"",
      ""html_url"": ""https://github.com/octocat"",
      ""followers_url"": ""https://api.github.com/users/octocat/followers"",
      ""following_url"": ""https://api.github.com/users/octocat/following{/other_user}"",
      ""gists_url"": ""https://api.github.com/users/octocat/gists{/gist_id}"",
      ""starred_url"": ""https://api.github.com/users/octocat/starred{/owner}{/repo}"",
      ""subscriptions_url"": ""https://api.github.com/users/octocat/subscriptions"",
      ""organizations_url"": ""https://api.github.com/users/octocat/orgs"",
      ""repos_url"": ""https://api.github.com/users/octocat/repos"",
      ""events_url"": ""https://api.github.com/users/octocat/events{/privacy}"",
      ""received_events_url"": ""https://api.github.com/users/octocat/received_events"",
      ""type"": ""User"",
      ""site_admin"": false
    },
    ""assets"": [
      {
        ""url"": ""https://api.github.com/repos/octocat/Hello-World/releases/assets/1"",
        ""browser_download_url"": ""https://github.com/octocat/Hello-World/releases/download/v1.0.0/example.zip"",
        ""id"": 1,
        ""name"": ""example.zip"",
        ""label"": ""short description"",
        ""state"": ""uploaded"",
        ""content_type"": ""application/zip"",
        ""size"": 1024,
        ""download_count"": 42,
        ""created_at"": ""2013-02-27T19:35:32Z"",
        ""updated_at"": ""2013-02-27T19:35:32Z"",
        ""uploader"": {
          ""login"": ""octocat"",
          ""id"": 1,
          ""avatar_url"": ""https://github.com/images/error/octocat_happy.gif"",
          ""gravatar_id"": """",
          ""url"": ""https://api.github.com/users/octocat"",
          ""html_url"": ""https://github.com/octocat"",
          ""followers_url"": ""https://api.github.com/users/octocat/followers"",
          ""following_url"": ""https://api.github.com/users/octocat/following{/other_user}"",
          ""gists_url"": ""https://api.github.com/users/octocat/gists{/gist_id}"",
          ""starred_url"": ""https://api.github.com/users/octocat/starred{/owner}{/repo}"",
          ""subscriptions_url"": ""https://api.github.com/users/octocat/subscriptions"",
          ""organizations_url"": ""https://api.github.com/users/octocat/orgs"",
          ""repos_url"": ""https://api.github.com/users/octocat/repos"",
          ""events_url"": ""https://api.github.com/users/octocat/events{/privacy}"",
          ""received_events_url"": ""https://api.github.com/users/octocat/received_events"",
          ""type"": ""User"",
          ""site_admin"": false
        }
      }
    ]
  }
]";
            string cSharpString = JsonCSharpGenerator.FromJsonArray(input).Replace("\r\n", "\n");
            string expected = @"public class Author
{
    public string Login { get;set; }
    public int Id { get;set; }
    public string AvatarUrl { get;set; }
    public DateTime GravatarId { get;set; }
    public string Url { get;set; }
    public string HtmlUrl { get;set; }
    public string FollowersUrl { get;set; }
    public string FollowingUrl { get;set; }
    public string GistsUrl { get;set; }
    public string StarredUrl { get;set; }
    public string SubscriptionsUrl { get;set; }
    public string OrganizationsUrl { get;set; }
    public string ReposUrl { get;set; }
    public string EventsUrl { get;set; }
    public string ReceivedEventsUrl { get;set; }
    public string Type { get;set; }
    public bool SiteAdmin { get;set; }
}

public class Uploader
{
    public string Login { get;set; }
    public int Id { get;set; }
    public string AvatarUrl { get;set; }
    public DateTime GravatarId { get;set; }
    public string Url { get;set; }
    public string HtmlUrl { get;set; }
    public string FollowersUrl { get;set; }
    public string FollowingUrl { get;set; }
    public string GistsUrl { get;set; }
    public string StarredUrl { get;set; }
    public string SubscriptionsUrl { get;set; }
    public string OrganizationsUrl { get;set; }
    public string ReposUrl { get;set; }
    public string EventsUrl { get;set; }
    public string ReceivedEventsUrl { get;set; }
    public string Type { get;set; }
    public bool SiteAdmin { get;set; }
}

public class Assets
{
    public string Url { get;set; }
    public string BrowserDownloadUrl { get;set; }
    public int Id { get;set; }
    public string Name { get;set; }
    public string Label { get;set; }
    public string State { get;set; }
    public string ContentType { get;set; }
    public int Size { get;set; }
    public int DownloadCount { get;set; }
    public DateTime CreatedAt { get;set; }
    public DateTime UpdatedAt { get;set; }
    public Uploader Uploader { get;set; }
}

public class RootObject
{
    public string Url { get;set; }
    public string HtmlUrl { get;set; }
    public string AssetsUrl { get;set; }
    public string UploadUrl { get;set; }
    public string TarballUrl { get;set; }
    public string ZipballUrl { get;set; }
    public int Id { get;set; }
    public string TagName { get;set; }
    public string TargetCommitish { get;set; }
    public string Name { get;set; }
    public string Body { get;set; }
    public bool Draft { get;set; }
    public bool Prerelease { get;set; }
    public DateTime CreatedAt { get;set; }
    public DateTime PublishedAt { get;set; }
    public Author Author { get;set; }
    public List<Assets> Assets { get;set; }
}".Replace("\r\n", "\n");
            Assert.That(cSharpString, Is.Not.Null);
            Assert.That(cSharpString.Length, Is.GreaterThan(0));
            Assert.That(cSharpString.Trim(), Is.EqualTo(expected.Trim()));
        }

        [Test]
        public void CanGeneratorSingleStringProperty()
        {
            string json = "{\r\n   \"hello\": \"zz\"\r\n}";

            var result = JsonCSharpGenerator.FromJsonObject(json);
            string expected = @"public class RootObject
{
    public string Hello { get;set; }
}";

            Assert.AreEqual(expected,result.Trim());
        }
    }
}
