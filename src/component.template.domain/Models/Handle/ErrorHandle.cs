using System;
using component.template.domain.Interfaces.Handle;
using component.template.domain.Models.Common;

namespace component.template.domain.Models.Handle;

 public class ErrorHandle : IErrorHandle
 {
     public List<BaseError> Errors { get; set; }  = new List<BaseError>();

     public void AddError(Exception error, int? statusCode = null) 
         => Errors.Add(
             new() 
             { 
                 Message = error.Message,
                 Source = error.Source,
                 Type = error.GetType().FullName,
                 StatusCode = statusCode
             });
 }
