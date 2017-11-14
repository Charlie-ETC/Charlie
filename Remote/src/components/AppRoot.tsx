import * as React from 'react';
import { Route, Switch } from 'react-router';
import { Link } from 'react-router-dom';
import { Toolbar, ToolbarRow, ToolbarTitle, ToolbarSection } from 'rmwc/Toolbar';
import { PersistentDrawer, PersistentDrawerContent } from 'rmwc/Drawer';
import { List, ListItem, ListItemStartDetail, ListItemText } from 'rmwc/List';
import { Icon, IconButton } from 'rmwc/IconButton';

import { GameObjectsPage } from './pages/GameObjectsPage';
import './AppRoot.css';

export interface AppRootProps { }
export interface AppRootStates {
    drawerOpen?: boolean
}

// 'HelloProps' describes the shape of props.
// State is never set so we use the '{}' type.
export class AppRoot extends React.Component<AppRootProps, AppRootStates> {
    constructor(props: AppRootProps) {
        super(props);
        this.state = {
            drawerOpen: true
        };
    }

    render() {
        return <div>
            <Toolbar>
                <ToolbarRow>
                    <ToolbarSection alignStart>
                        <IconButton style={{ color: 'inherit', alignSelf: 'center' }}
                            onClick={() => this.setState({ drawerOpen: !this.state.drawerOpen })}>
                            menu
                        </IconButton>
                        <ToolbarTitle>Charlie Remote</ToolbarTitle>
                    </ToolbarSection>
                </ToolbarRow>
            </Toolbar>
            <div style={{ display: 'flex', flex: '1 1' }}>
                <PersistentDrawer open={this.state.drawerOpen}>
                    <PersistentDrawerContent>
                        <List>
                            <ListItem ripple>
                                <Link to='/gameObjects'>
                                    <ListItemText>
                                        GameObjects
                                    </ListItemText>
                                </Link>
                            </ListItem>
                            <ListItem ripple>
                                <ListItemText>
                                    Scenes
                                </ListItemText>
                            </ListItem>
                        </List>
                    </PersistentDrawerContent>
                </PersistentDrawer>
                <Switch>
                    <Route path="/" component={GameObjectsPage}></Route>
                    <Route path="/gameObjects" component={GameObjectsPage}></Route>
                </Switch>
            </div>
        </div>;
    }
}
