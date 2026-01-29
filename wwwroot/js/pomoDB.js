const dbPromise = idb.openDB('pomodoro-db', 1, {
    upgrade(db) {
        db.createObjectStore('state');
    },
});

export async function setPomoState(key, value) {
    const db = await dbPromise;
    await db.put('state', value, key);
}

export async function getPomoState(key) {
    const db = await dbPromise;
    return await db.get('state', key);
}
