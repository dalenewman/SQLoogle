using System.Collections.Generic;
using Sqloogle.Libs.Rhino.Etl.Core;
using Sqloogle.Libs.Rhino.Etl.Core.Operations;

namespace Tests {

    public class FakeOperation : AbstractOperation {

        private readonly List<Row> _rows = new List<Row>();

        public FakeOperation(IEnumerable<Row> rows ) {
            _rows.AddRange(rows);
        }

        public FakeOperation(params Row[] rows) {
            _rows.AddRange(rows);
        }

        public FakeOperation Add(IEnumerable<Row> rows ) {
            _rows.AddRange(rows);
            return this;
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            return _rows;
        }

    }
}
