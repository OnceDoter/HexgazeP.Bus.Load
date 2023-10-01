using HexgazeP.Common.Bus;
using MassTransit.Configuration;

namespace MassTransit.ActivityTracing
{
   public class ActivityTracingPipeSpecification : IPipeSpecification<ConsumeContext>, IPipeSpecification<PublishContext>, IPipeSpecification<SendContext>
   {
      public IEnumerable<ValidationResult> Validate()
      {
         return Enumerable.Empty<ValidationResult>();
      }

      public void Apply(IPipeBuilder<ConsumeContext> builder)
      {
         builder.AddFilter(new ActivityTracingConsumeFilter());
      }

      public void Apply(IPipeBuilder<PublishContext> builder)
      {
         builder.AddFilter(new OpenTracingPublishFilter());
      }

      public void Apply(IPipeBuilder<SendContext> builder)
      {
         builder.AddFilter(new OpenTracingPublishFilter());
      }
   }
}
