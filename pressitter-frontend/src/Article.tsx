import * as React from 'react';

export default class Article extends React.Component {

  constructor(props) {
    super(props);
    this.state = {
      TitleEnglish: "Test Title3"
    };
  }

  state = {
    TitleEnglish: "Test Title2"
  };

  // increment = () => {
  //   this.setState({
  //     count: (this.state.count += 1)
  //   });
  // };

  // decrement = () => {
  //   this.setState({
  //     count: (this.state.count -= 1)
  //   });
  // };

  render () {
    return (
      <div>
        <ul className='story-container list-group-item list-group-item-action'>
          <li className='story-item story-icons'>
            <div>
              <div style={{marginBottom: 5}}>
                <img src={this.props.CountryUrl} style={{width: 30}}/>
              </div>
              <div>
                <img src={this.props.LogoUrl} style={{width: 30}}/>
              </div>
            </div>
          </li>
          <li className='story-item story-text'>
            <a target='_blank' href={"http://translate.google.com/translate?js=n&sl=auto&tl=en&u=" + this.props.StoryUrl} role='button'>{this.props.TitleEnglish}</a>
          </li>
        </ul>        
      </div>
    );
  }
}