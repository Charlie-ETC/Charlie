import * as React from 'react';
import { List, ListItem, ListItemStartDetail, ListItemText } from 'rmwc/List';
import axios from 'axios';

interface GameObject {
    id: string,
    name: string
}

interface GameObjectsPageState {
    gameObjects: GameObject[]
}

export class GameObjectsPage extends React.Component<{}, GameObjectsPageState> {
    constructor(props: {}) {
        super(props);
        this.state = { gameObjects: [] };
        this.getGameObjects();
    }

    async getGameObjects() {
        const response = await axios.get('http://localhost:24275/api/gameObjects');
        this.setState({
            gameObjects: response.data
        });
    }

    render() {
        return <div>
            <List>
                {this.state.gameObjects.map(gameObject =>
                    <ListItem key={gameObject.id}>
                        <ListItemText>{gameObject.name}</ListItemText>
                    </ListItem>
                )}
            </List>
        </div>;
    }
}
