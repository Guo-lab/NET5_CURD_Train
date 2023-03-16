using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ESC5.Domain.DomainModel.TR;
using Microsoft.AspNetCore.Mvc;
using ProjectBase.Domain;
using static ESC5.Common.ViewModel.TR.TaskEditVM;

namespace ESC5.Common.ViewModel.TR
{
   
    [Bind("Input")]
    public class TaskMultiEditVM
    {
        public MultiEditInput Input { get; set; } = new();
        public IList<DORef<User, int>> UserList { get; set; }
        public EditInput DummyRow { get; set; }

        public class MultiEditInput
        {
            public IList<EditInput> Rows { get; set; } = new List<EditInput>();
        }
      
    }
}


