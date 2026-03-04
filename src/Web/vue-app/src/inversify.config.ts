import {Container} from "inversify";
import axios, {AxiosInstance} from 'axios';
import "reflect-metadata";

import {TYPES} from "@/injection/types";
import {
  IAdministratorService,
  IApiService,
  IAuthenticationService,
  IBookService,
  IMemberService,
  IModulesService,
  IUserService,
  IConversationService
} from "@/injection/interfaces";
import {
  ApiService,
  AuthenticationService,
  BookService,
  MemberService,
  ModulesApiService,
  UserService,
  ConversationService
} from "@/services";
import {AdministratorService} from "@/services/administratorService";

const dependencyInjection = new Container();
dependencyInjection.bind<AxiosInstance>(TYPES.AxiosInstance).toConstantValue(axios.create({ withCredentials: true }))
dependencyInjection.bind<IApiService>(TYPES.IApiService).to(ApiService).inSingletonScope()
dependencyInjection.bind<IAdministratorService>(TYPES.IAdministratorService).to(AdministratorService).inSingletonScope()
dependencyInjection.bind<IAuthenticationService>(TYPES.IAuthenticationService).to(AuthenticationService).inSingletonScope()
dependencyInjection.bind<IBookService>(TYPES.IBookService).to(BookService).inSingletonScope()
dependencyInjection.bind<IMemberService>(TYPES.IMemberService).to(MemberService).inSingletonScope()
dependencyInjection.bind<IModulesService>(TYPES.IModulesService).to(ModulesApiService).inSingletonScope()
dependencyInjection.bind<IUserService>(TYPES.IUserService).to(UserService).inSingletonScope()
dependencyInjection.bind<IConversationService>(TYPES.IConversationService).to(ConversationService).inSingletonScope()

function useAdministratorService() {
  return dependencyInjection.get<IAdministratorService>(TYPES.IAdministratorService);
}

function useAuthenticationService() {
  return dependencyInjection.get<IAuthenticationService>(TYPES.IAuthenticationService);
}

function useMemberService() {
  return dependencyInjection.get<IMemberService>(TYPES.IMemberService);
}

function useBookService() {
  return dependencyInjection.get<IBookService>(TYPES.IBookService);
}

function useModulesService() {
  return dependencyInjection.get<IModulesService>(TYPES.IModulesService);
}

function useUserService() {
  return dependencyInjection.get<IUserService>(TYPES.IUserService);
}

function useConversationService() {
  return dependencyInjection.get<IConversationService>(TYPES.IConversationService);
}


export {
  dependencyInjection,
  useAdministratorService,
  useAuthenticationService,
  useBookService,
  useMemberService,
  useModulesService,
  useUserService,
  useConversationService
};