import * as React from 'react';
import { render } from 'react-dom';

import Article from './Article';
import ArticlesCurrent from './ArticlesCurrent';
import Header from './Header';

render(<Header/>, document.getElementById('header'));
render(<ArticlesCurrent/>, document.getElementById('main'));