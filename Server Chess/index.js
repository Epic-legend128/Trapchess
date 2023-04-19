const express = require('express');
const app = express();
const port = 3000;

app.get('/', (req, res) => {
    res.send('Hello, World');
})

try {
    app.listen(port, () => {
        console.log(`[STARTED] -> Running on port ${port}`);
    })
}
catch (err) {
    console.log(`[ERROR] -> ${err.name + ":     " + err.message} `);
}