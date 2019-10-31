using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;

namespace CallCenter_BLL.Utilities.AutoMapper.AttributeMapping
{
  /// <summary>
  /// This class is the bootstrapper for the data annotation. It replaces the current meta data provider with a mapped one.
  /// Register a new MetadataProvider that map Model to ViewModel metadata. This is used to keep the business logic from
  /// attribute into the model side but still have them transfered for JQuery Validation.
  /// </summary>
  public class ModelMetadataProviderConfig
  {
    public static void RegisterModelMetadataProvider()
    {
      ModelMetadataProviders.Current = new MappedDataAnnotationsModelMetadataProvider(Mapper.Configuration);
    }
  }
}
