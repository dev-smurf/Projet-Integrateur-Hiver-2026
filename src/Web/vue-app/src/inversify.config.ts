import { Container } from "inversify";
import axios, { AxiosInstance } from "axios";
import "reflect-metadata";

import { TYPES } from "@/injection/types";
import {
  IAdministratorService,
  IApiService,
  IAuthenticationService,
  IBookService,
  IEquipesService,
  IMemberService,
  IModulesService,
  IUserService,
  IConversationService,
  IEquipeConversationService,
  IAppointmentService,
  IQuizService
} from "@/injection/interfaces";
import {
  ApiService,
  AuthenticationService,
  BookService,
  MemberService,
  ModulesApiService,
  UserService,
  ConversationService,
  EquipeConversationService,
  AppointmentService,
  QuizService
} from "@/services";
import { AdministratorService } from "@/services/administratorService";
import { EquipeService } from "./services/equipeService";
import { NotesService, type INotesService } from "@/services/NotesService";

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
dependencyInjection.bind<IEquipeConversationService>(TYPES.IEquipeConversationService).to(EquipeConversationService).inSingletonScope()
dependencyInjection.bind<IAppointmentService>(TYPES.IAppointmentService).to(AppointmentService).inSingletonScope()
dependencyInjection
  .bind<IEquipesService>(TYPES.IEquipesService)
  .to(EquipeService)
  .inSingletonScope();
dependencyInjection.bind<IQuizService>(TYPES.IQuizService).to(QuizService).inSingletonScope();
dependencyInjection.bind<INotesService>(TYPES.INotesService).to(NotesService).inSingletonScope();

function useAdministratorService() {
  return dependencyInjection.get<IAdministratorService>(
    TYPES.IAdministratorService,
  );
}

function useAuthenticationService() {
  return dependencyInjection.get<IAuthenticationService>(
    TYPES.IAuthenticationService,
  );
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

function useEquipesService() {
  return dependencyInjection.get<IEquipesService>(TYPES.IEquipesService);
}

function useUserService() {
  return dependencyInjection.get<IUserService>(TYPES.IUserService);
}

function useConversationService() {
  return dependencyInjection.get<IConversationService>(TYPES.IConversationService);
}

function useEquipeConversationService() {
  return dependencyInjection.get<IEquipeConversationService>(TYPES.IEquipeConversationService);
}

function useAppointmentService() {
  return dependencyInjection.get<IAppointmentService>(TYPES.IAppointmentService);
}

function useQuizService() {
  return dependencyInjection.get<IQuizService>(TYPES.IQuizService);
}

function useNotesService() {
  return dependencyInjection.get<INotesService>(TYPES.INotesService);
}

export {
  dependencyInjection,
  useAdministratorService,
  useAuthenticationService,
  useBookService,
  useMemberService,
  useModulesService,
  useUserService,
  useConversationService,
  useEquipeConversationService,
  useAppointmentService,
  useEquipesService,
  useQuizService,
  useNotesService
};
