import * as React from 'react';
import * as ReactDOM from 'react-dom';
import { BrowserRouter } from 'react-router-dom';

import { AppRoot } from './components/AppRoot';

ReactDOM.render(
    <BrowserRouter>
        <AppRoot />
    </BrowserRouter>,
    document.getElementById('root')
);
