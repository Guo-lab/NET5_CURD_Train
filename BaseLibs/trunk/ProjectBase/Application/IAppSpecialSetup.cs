namespace ProjectBase.Application {
    /// <summary>
    /// 特定类型的应用程序需要做的初始化工作
    /// </summary>
    public interface IAppSpecialSetup : IAppSetup {
         void SetupSpecialFeature();
    }
}
