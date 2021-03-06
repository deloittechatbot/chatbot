﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.Bot.Schema;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using TestAssistant1.Models;

namespace TestAssistant1.Tests
{
    [TestClass]
    [TestCategory("UnitTests")]
    public class OnboardingDialogTests : BotTestBase
    {
        [TestMethod]
        public async Task Test_Onboarding_Flow()
        {
            var testName = "Jane Doe";

            var profileState = new UserProfileState();
            profileState.Name = testName;

            var allNamePromptVariations = LocaleTemplateEngine.TemplateEnginesPerLocale[CultureInfo.CurrentUICulture.Name].ExpandTemplate("NamePrompt");
            var allHaveMessageVariations = LocaleTemplateEngine.TemplateEnginesPerLocale[CultureInfo.CurrentUICulture.Name].ExpandTemplate("HaveNameMessage", profileState);

            dynamic data = new JObject();
            data.name = testName;

            await GetTestFlow(includeUserProfile: false)
                .Send(new Activity()
                {
                    Type = ActivityTypes.ConversationUpdate,
                    MembersAdded = new List<ChannelAccount>() { new ChannelAccount("user") }
                })
                .AssertReply(activity => Assert.AreEqual(1, activity.AsMessageActivity().Attachments.Count))
                .AssertReplyOneOf(allNamePromptVariations.ToArray())
                .Send(testName)
                .AssertReplyOneOf(allHaveMessageVariations.ToArray())
                .StartTestAsync();
        }
    }
}
