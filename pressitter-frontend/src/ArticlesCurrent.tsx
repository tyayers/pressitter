import * as React from 'react';

import Article from './Article';

import * as testdata from './testdata2.json';
import ca from './img/ca.png';
import us from './img/us.png';
import de from './img/de.png';
import fr from './img/fr.png';

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

    this.setState({news: testdata});

    fetch("src/testdata.json")
      .then(response => response.json())
      .then(data => this.setState({ news: testdata }));
  }

  render () {


    return (
      <div>
        <h1>{this.state.TitleEnglish}</h1>
        <h2>{this.state.news.Date}</h2> */}
        {this.state.news.Topics.map((topic, index) => (
          <div key={index}>
            <h3
            >{topic.Topic}</h3>
            {topic.News.map((newsitem, i) => (
              <div key={i}>
                <Article TitleEnglish={newsitem.TitleEnglish} CountryUrl={images[newsitem.Region]} LogoUrl={images[newsitem.Source.toLowerCase().replace(" ", "")]} StoryUrl={newsitem.Url}></Article>
              </div>
            ))}
          </div>
        ))}
      </div>
    );
  }
}