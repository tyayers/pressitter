import * as React from 'react';

import logo from './img/pressitter.png';

export default class Header extends React.Component {

  constructor(props) {
    super(props);
    this.state = {
      TitleEnglish: "Test Title3",
      news: {Date: "Loading..", Topics: []},
    };
  }

  state = {
    TitleEnglish: "Test Title2",
    news: null,
  };

  componentDidMount() {


  }

  render () {


    return (
        <nav className="navbar navbar-light bg-light">
          <a className="navbar-brand" href="#">
            <img src={logo} width="30" height="30" className="d-inline-block align-top" alt=""/>
            <i>ressitter</i>
          </a>
        </nav>  
    );
  }
}