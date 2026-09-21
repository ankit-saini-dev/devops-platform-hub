export interface ILoginRequest {
  identifier: string;
  password: string;
}

export interface IRegistrationRequest {
  name: string;
  username: string;
  email: string;
  password: string;
}

export interface IAuthenticationResponse {
  accessToken: string;
  expiresAt: string;
  name: string;
  username: string;
  roles: readonly string[];
}

export interface ICurrentUser {
  id: string;
  username: string;
  name: string;
  roles: readonly string[];
}

export interface IAuthenticationSession {
  accessToken: string;
  expiresAt: string;
  currentUser: ICurrentUser;
}
