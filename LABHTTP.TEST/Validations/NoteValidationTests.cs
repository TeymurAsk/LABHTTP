using LABHTTP.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LABHTTP.TEST.Validations
{
    public class NoteValidationTests
    {
        [Fact]
        public void Note_WithEmptyTitle_IsInvalid()
        {
            var note = new Note { Title = "" };
            var ctx = new ValidationContext(note);
            var results = new List<ValidationResult>();

            var valid = Validator.TryValidateObject(note, ctx, results, true);

            Assert.False(valid);
        }
    }
}
