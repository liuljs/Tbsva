using AutoMapper;
using WebShopping.Helpers;
using WebShopping.Profiles;
using WebShopping.Services;
using System;

using Unity;

namespace WebShopping
{
    /// <summary>
    /// Specifies the Unity configuration for the main container.
    /// </summary>
    public static class UnityConfig
    {
        #region Unity Container
        private static Lazy<IUnityContainer> container =
          new Lazy<IUnityContainer>(() =>
          {
              var container = new UnityContainer();
             
              //µù¥UauotmapperÃþ§O
              IMapper mapper = RegisterMapper();
              container.RegisterInstance(mapper);

              RegisterTypes(container);
              return container;
          });

        /// <summary>
        /// Configured Unity Container.
        /// </summary>
        public static IUnityContainer Container => container.Value;
        #endregion

        /// <summary>
        /// Registers the type mappings with the Unity container.
        /// </summary>
        /// <param name="container">The unity container to configure.</param>
        /// <remarks>
        /// There is no need to register concrete types such as controllers or
        /// API controllers (unless you want to change the defaults), as Unity
        /// allows resolving a concrete type even if it was not previously
        /// registered.
        /// </remarks>
        public static void RegisterTypes(IUnityContainer container)
        {
            // NOTE: To load from web.config uncomment the line below.
            // Make sure to add a Unity.Configuration to the using statements.
            // container.LoadConfiguration();

            // TODO: Register your type's mappings here.
            // container.RegisterType<IProductRepository, ProductRepository>();

            // Multi class of an Interface
            //container.RegisterType<IFoo, Foo1>("Foo1");
            //container.RegisterType<IFoo, Foo2>("Foo2");
            //container.RegisterType<Client1>(new InjectionConstructor(new ResolvedParameter<IFoo>("Foo1")));
            //container.RegisterType<Client2>(new InjectionConstructor(new ResolvedParameter<IFoo>("Foo2")));
            //Client1 client1 = container.Resolve<Client1>();
            //Client2 client2 = container.Resolve<Client2>();

            container.RegisterType<IIndexSlideshowService, IndexSlideshowService>();
            container.RegisterType<IAboutMeService, AboutMeService>();
            container.RegisterType<IPrivacyService, PrivacyService>();
            container.RegisterType<ITermsService, TermsService>();
            container.RegisterType<INewsService, NewsService>();
            container.RegisterType<IFaqService, FaqService>();
            container.RegisterType<IImageFormatHelper, ImageFormatHelper>();
            container.RegisterType<IImageFileHelper, ImageFileHelper>();
            container.RegisterType<IDapperHelper, DapperHelper>();
            container.RegisterType<IMemberService, MemberService>();
            container.RegisterType<IProductService, ProductService>();
            container.RegisterType<ICategoryService, CategoryService>();
            container.RegisterType<IOrdersService, OrdersService>();
            container.RegisterType<ISpecService, SpecService>();
            container.RegisterType<IPaymentService, PaymentService>();
            container.RegisterType<IArticleCategoryService, ArticleCategoryService>();
            container.RegisterType<IArticleContentService, ArticleContentService>();
            container.RegisterType<ILightingCategoryService, LightingCategoryService>();
            container.RegisterType<ILightingContentService, LightingContentService>();
            container.RegisterType<IKnowledgeService, KnowledgeService>();
            container.RegisterType<IKnowledge2Service, Knowledge2Service>();
            container.RegisterType<IQAService, QAService>();
            container.RegisterType<IPaymentMailingService, PaymentMailingService>();
            container.RegisterType<IPictureListService, PictureListService>();
            container.RegisterType<IOtherAccessoriesService, OtherAccessoriesService>();
            container.RegisterType<IOther1Service, Other1Service>();
            container.RegisterType<IVideoCategoryService, VideoCategoryService>();
            container.RegisterType<IVideoContentService, VideoContentService>();
            container.RegisterType<IDonateService, DonateService>();
            container.RegisterType<IDonateRelatedItemService, DonateRelatedItemService>();
            container.RegisterType<IPaymentDonateService, PaymentDonateService>();
            container.RegisterType<IActivityCategoryService, ActivityCategoryService>();
            container.RegisterType<IActivityService, ActivityService>();
            container.RegisterType<IVideoService, VideoService>();
            container.RegisterType<IVideo2Service, Video2Service>();
            container.RegisterType<ITimeMachineService, TimeMachineService>();
            container.RegisterType<IDirectorIntroductionService, DirectorIntroductionService>();
            container.RegisterType<ITaiwanDojoCategoryService, TaiwanDojoCategoryService>();
            container.RegisterType<ITaiwanDojoService, TaiwanDojoService>();
            container.RegisterType<IRepeatLimit, RepeatLimit>();
            container.RegisterType<ITextEditorService, TextEditorService>();
            container.RegisterType<IConvertFormsService, ConvertFormsService>();
            container.RegisterType<IAddTbsvaService, AddTbsvaService>();
            container.RegisterType<ILotusService, LotusService>();
            container.RegisterType<IGraphicsEditorService, GraphicsEditorService>();
            container.RegisterType<IAuthAdminV2Service, AuthAdminV2Service>();
        }

        public static IMapper RegisterMapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                //Create all maps here
                cfg.AddProfile<CategoryAdminProfile>();
                cfg.AddProfile<OrdersAdminProfile>();
                cfg.AddProfile<IndexSlideshowProfile>();
                cfg.AddProfile<NewsProfile>();
                cfg.AddProfile<FaqProfile>();
                cfg.AddProfile<ProductAdminProfile>();
                cfg.AddProfile<PaymentProfile>();
                cfg.AddProfile<ArticleCategoryListProfile>();
                cfg.AddProfile<ArticleContentListProfile>();
                cfg.AddProfile<LightingCategoryListProfile>();
                cfg.AddProfile<LightingContentListProfile>();
                cfg.AddProfile<KnowledgeContentListProfile>();
                cfg.AddProfile<KnowledgeContent2ListProfile>();
                cfg.AddProfile<QAListProfile>();
                cfg.AddProfile<PaymentMailingProfile>();
                cfg.AddProfile<PictureListProfile>();
                cfg.AddProfile<OtherAccessoriesProfile>();
                cfg.AddProfile<Other1Profile>();
                cfg.AddProfile<VideoCategoryProfile>();
                cfg.AddProfile<VideoContentProfile>();
                cfg.AddProfile<DonateProfile>();
                cfg.AddProfile<ActivityCategoryProfile>();
                cfg.AddProfile<ActivityProfile>();
                cfg.AddProfile<VideoProfile>();
                cfg.AddProfile<Video2Profile>();
                cfg.AddProfile<TimeMachineProfile>();
                cfg.AddProfile<DirectorIntroductionProfile>();
                cfg.AddProfile<TaiwanDojoCategoryProfile>();
                cfg.AddProfile<TaiwanDojoProfile>();
                cfg.AddProfile<TextEditorProfile>();
                cfg.AddProfile<ConvertFormsProfile>();
                cfg.AddProfile<AddTbsvaProfile>();
                cfg.AddProfile<LotusProfile>();
                cfg.AddProfile<GraphicsEditorProfile>();
                cfg.AddProfile<ClassificationModuleV2Profile>();
            });

            IMapper mapper = config.CreateMapper();

            return mapper;
        }
    }
}