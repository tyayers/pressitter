import * as React from 'react';

import Article from './Article';

import * as testdata from './testdata2.json';
// import ca from './img/ca.png';
// import us from './img/us.png';
// import de from './img/de.png';
// import fr from './img/fr.png';

import images from './img/*.png';

export default class ArticlesCurrent extends React.Component {

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

    if (process.env.NODE_ENV === 'development') 
    { 
      this.setState({news: testdata});
    }
    else {
      fetch("https://pressitter-functions.azurewebsites.net/api/GetTodaysNews?code=ypUwQNr/R2kkvTUQg9j3DHCGZJ89xbqrJxoog6HxsrTfPrnb7pgSZA==")
        .then(response => response.json())
        .then((data) => this.setState({ news: data }));   
    }
  }

  getGreeting() {
    var myDate = new Date();
    var result = "Good Morning!";

    if (myDate.getHours() > 12 && myDate.getHours() < 18)
      result = "Good Afternoon!";
    else if (myDate.getHours() >= 18)
      result = "Good Evening!";

    return result;
  }

  getFormattedDate(date) {
    var pieces = date.split(".");
    var newDate = new Date(pieces[2] + "-" + pieces[1] + "-" + pieces[0]);

    return newDate.toDateString();
  }  

  render () {


    return (
      <div>
        <div className="jumbotron">
          <div className="container">
            <h1 className="display-4">{this.getGreeting()}</h1>
            <p className="lead">Here are the topics from the worldwide news for {this.getFormattedDate(this.state.news.Date)}.</p>
          </div>
        </div>        
        <div className="container">
          <div className="row">
          {this.state.news.Topics.map((topic, index) => (
            <div className="col-12 topic-collection" key={index}>
              <ul className="list-group">
                <li className="list-group-item active">{topic.Topic}</li>
              {topic.News.map((newsitem, i) => (
                <div key={i}>
                  <li className="list-group-item">
                    <Article TitleEnglish={newsitem.TitleEnglish} CountryUrl={images[newsitem.Region]} LogoUrl={images[newsitem.Source.toLowerCase().replace(" ", "")]} StoryUrl={newsitem.Url}></Article>
                  </li>
                </div>
              ))}
              </ul>
            </div>
          ))}
          </div>
        </div>
      </div>
    );
  }
}