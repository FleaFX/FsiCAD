type DotNetReference = {
    invokeMethodAsync<T>(methodName: string, ...args: any[]): Promise<T>,
    dispose(): void
}

export type IndexedDbDatabase = {
    openCursor: (objectStoreName: string, callback: DotNetReference) => void
};

export const open = (name: string, version?: number | undefined, storeName?: string | undefined): Promise<IndexedDbDatabase> =>
    new Promise<IndexedDbDatabase>(
        (resolve, reject) => {
            const req = window.indexedDB.open(name, version);
            req.onsuccess = async (event: Event) => {
                const db = (<IDBOpenDBRequest>event.target).result;
                resolve(({
                    openCursor: (objectStoreName: string, callback: DotNetReference): void => {
                        const tx = db.transaction(objectStoreName, "readonly");
                        const store = tx.objectStore(objectStoreName);
                        const cursor = store.openCursor();
                        cursor.onsuccess = (event: Event) => {
                            const cursor = (<IDBRequest<IDBCursorWithValue>>event.target).result;
                            if (cursor) {
                                callback.invokeMethodAsync("YieldAsync", cursor.value);
                                cursor.continue();
                            } else {
                                callback.invokeMethodAsync<void>("BreakAsync");
                                callback.dispose();
                            }
                        };
                    }
                }));
            };

            req.onerror = (_) => {
                reject(req.error?.message);
            };

            req.onupgradeneeded = (event: Event) => {
                if (storeName) {
                    const db = ((<IDBOpenDBRequest>event.target).result);
                    db.createObjectStore(storeName, { autoIncrement: true });
                }
            };
        }
    );
