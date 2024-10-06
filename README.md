# SqliteNetRowsChangedIssue

This repository is a reproduction of an SQlite-Net bug where ExecuteAsync and ExecuteAsync and RunInTransactionAsync are not safe together. See https://github.com/praeclarum/sqlite-net/issues/1250

This seems to occur because sqlite-net is doing the transaction and the ExectueAsync on the same connection but from different threads. When you begin the transaction using RunInTransactionAsync then the ExecuteAsync call ends up using that transaction if it runs before the transaction ends (its the same connection). So calling sqlite3_changes gets the value from the update.