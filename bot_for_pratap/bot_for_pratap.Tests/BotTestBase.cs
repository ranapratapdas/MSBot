using System.Collections.Generic;
using System.Threading;
using Autofac;
using bot_for_pratap;
using bot_for_pratap.Dialogs.Onboarding;
using bot_for_pratap.Middleware.Telemetry;
using bot_for_pratap.Tests.LuisTestUtils;
using bot_for_pratap.Tests.LUTestUtils;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Adapters;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace bot_for_pratap.Tests
{
    public class BotTestBase
    {
        public IContainer Container { get; set; }

        public BotServices BotServices { get; set; }

        public ConversationState ConversationState { get; set; }

        public UserState UserState { get; set; }

        public IBotTelemetryClient TelemetryClient { get; set; }

        [TestInitialize]
        public virtual void Initialize()
        {
            var builder = new ContainerBuilder();

            ConversationState = new ConversationState(new MemoryStorage());
            UserState = new UserState(new MemoryStorage());
            TelemetryClient = new NullBotTelemetryClient();
            BotServices = new BotServices()
            {
                DispatchRecognizer = DispatchTestUtil.CreateRecognizer(),
                LuisServices = new Dictionary<string, ITelemetryLuisRecognizer>
                {
                    { "general", GeneralTestUtil.CreateRecognizer() }
                },
                QnAServices = new Dictionary<string, ITelemetryQnAMaker>
                {
                    { "faq", FaqTestUtil.CreateRecognizer() },
                    { "chitchat", ChitchatTestUtil.CreateRecognizer() }
                }
            };

            builder.RegisterInstance(new BotStateSet(UserState, ConversationState));
            Container = builder.Build();
        }

        public TestFlow GetTestFlow()
        {
            var adapter = new TestAdapter()
                .Use(new AutoSaveStateMiddleware(UserState, ConversationState));

            var testFlow = new TestFlow(adapter, async (context, token) =>
            {
                var bot = BuildBot();
                await bot.OnTurnAsync(context, CancellationToken.None);
            });

            return testFlow;
        }

        public IBot BuildBot()
        {
            return new bot_for_pratap.Bot(BotServices, ConversationState, UserState, TelemetryClient);
        }
    }
}
