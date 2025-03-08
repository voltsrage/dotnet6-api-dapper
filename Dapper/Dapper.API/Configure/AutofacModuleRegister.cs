using Autofac;

namespace Dapper.API.Configure
{
    public class AutofacModuleRegister : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var asm = typeof(Program).Assembly;

            builder.RegisterAssemblyTypes(asm)
                  .Where(t => t.Name.EndsWith("Repository"))
                  .AsImplementedInterfaces();

            builder.RegisterAssemblyTypes(asm)
                  .Where(t => t.Name.EndsWith("Service"))
                  .AsImplementedInterfaces();
        }
    }
}
