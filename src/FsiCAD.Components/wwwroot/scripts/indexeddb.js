export const open = (name, version, storeName) => new Promise((resolve, reject) => {
    const req = window.indexedDB.open(name, version);
    req.onsuccess = async (event) => {
        const db = event.target.result;
        resolve(({
            openCursor: (objectStoreName, callback) => {
                const tx = db.transaction(objectStoreName, "readonly");
                const store = tx.objectStore(objectStoreName);
                const cursor = store.openCursor();
                cursor.onsuccess = (event) => {
                    const cursor = event.target.result;
                    if (cursor) {
                        callback.invokeMethodAsync("YieldAsync", cursor.value);
                        cursor.continue();
                    }
                    else {
                        callback.invokeMethodAsync("BreakAsync");
                        callback.dispose();
                    }
                };
            }
        }));
    };
    req.onerror = (_) => {
        reject(req.error?.message);
    };
    req.onupgradeneeded = (event) => {
        if (storeName) {
            const db = (event.target.result);
            db.createObjectStore(storeName, { autoIncrement: true });
        }
    };
});
//# sourceMappingURL=indexeddb.js.map