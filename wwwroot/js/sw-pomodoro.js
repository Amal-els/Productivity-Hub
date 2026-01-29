importScripts('https://unpkg.com/idb@7/build/iife/index-min.js');

const WORK_DURATION = 10;       // seconds for demo, change to minutes * 60
const SHORT_BREAK = 5;
const LONG_BREAK = 7;
const CYCLES_BEFORE_LONG_BREAK = 4;

// IndexedDB to store state
const dbPromise = idb.openDB('pomodoro-db', 1, {
    upgrade(db) {
        db.createObjectStore('state');
    },
});

async function getState(key) {
    const db = await dbPromise;
    return await db.get('state', key);
}

async function setState(key, value) {
    const db = await dbPromise;
    await db.put('state', value, key);
}

// Notifications
function showNotification(title, msg) {
    self.registration.showNotification(title, {
        body: msg,
        icon: '/favicon.ico'
    });
}

// Called every second
async function checkPomodoro() {
    const endTime = await getState('endTime');
    const type = await getState('type') || 'work';
    const cycles = parseInt(await getState('cycles') || '0');
    const now = Date.now();

    if (!endTime) return;

    const remaining = Math.floor((endTime - now) / 1000);

    if (remaining <= 0) {
        if (type === 'work') {
            let newCycles = cycles + 1;
            await setState('cycles', newCycles);

            if (newCycles % CYCLES_BEFORE_LONG_BREAK === 0) {
                await setState('type', 'longBreak');
                await setState('endTime', Date.now() + LONG_BREAK * 1000);
                showNotification('Pomodoro Timer', '🎉 Long Break! Take 15 minutes.');
            } else {
                await setState('type', 'shortBreak');
                await setState('endTime', Date.now() + SHORT_BREAK * 1000);
                showNotification('Pomodoro Timer', '☕ Short Break! Take 5 minutes.');
            }
        } else {
            await setState('type', 'work');
            await setState('endTime', Date.now() + WORK_DURATION * 1000);
            showNotification('Pomodoro Timer', '💪 Back to Work!');
        }

        // Notify all pages to update
        const clientsList = await self.clients.matchAll({ includeUncontrolled: true });
        for (const client of clientsList) {
            client.postMessage({ type: 'updateTimer' });
        }
    }
}

// Poll every second
setInterval(checkPomodoro, 1000);

self.addEventListener('install', e => self.skipWaiting());
self.addEventListener('activate', e => self.clients.claim());

// Receive messages from page
self.addEventListener('message', async (event) => {
    const msg = event.data;
    if (msg.type === 'startTimer') {
        await setState('type', msg.sessionType);
        await setState('endTime', msg.endTime);
    }
});

self.addEventListener('message', async (event) => {
    const msg = event.data;

    if (msg.type === 'startTimer') {
        await setState('type', msg.sessionType);
        await setState('endTime', msg.endTime);
    }

    if (msg.type === 'getState') {
        const type = await getState('type') || 'work';
        const endTime = await getState('endTime');
        const cycles = parseInt(await getState('cycles') || '0');

        let sessionStartTime = endTime ? endTime - (type === 'work' ? WORK_DURATION * 1000 : SHORT_BREAK * 1000) : null;
        let totalDuration = type === 'work' ? WORK_DURATION : SHORT_BREAK;

        event.source.postMessage({
            type: 'state',
            isWork: type === 'work',
            sessionStartTime,
            totalDuration,
            cycles
        });
    }
});
self.addEventListener('message', async (event) => {
    if (event.data?.type === 'startTimer') {
        await setState('endTime', event.data.endTime);
        await setState('type', event.data.sessionType);
    }
});


