#variables
PROJECT = ft_matcha
COMPOSE = docker compose
# colors

RESET = \e[0m
PURPLE = \033[0;35m
CYAN = \033[0;36m
YELLOW = \033[1;33m
RED = \033[0;31m
BLUE = \033[0;34m

# rules

build:
	@echo "${PURPLE}*Building ${PROJECT} environment...*${RESET}"
	@${COMPOSE} up -d --build

up:
	@echo "${CYAN}*Initializing ${PROJECT} setup...* ${RESET}"
	@${COMPOSE} up -d

down:
	@echo "${YELLOW}*Shutting down ${PROJECT}...* ${RESET}"
	@${COMPOSE} down

re: down
	@echo "${BLUE}* Rebuilding ${PROJECT}...* ${RESET}"
	@${COMPOSE} up -d --build

clean: down
	@echo "${RED}* Removing ${PROJECT} data...* ${RESET}"
	@docker system prune -a

fclean:
	@echo "${RED}* Performing complete clean-up...* ${RESET}"
	@if [ "$$(docker ps -qa)" != "" ]; then \
		docker stop $$(docker ps -qa); \
	fi
	@docker system prune --all --force --volumes
	@docker network prune --force
	@docker volume prune --force
	@if [ "$$(docker ps -qa)" != "" ]; then \
		docker rm $$(docker ps -qa); \
	fi
	@if [ "$$(docker images -qa)" != "" ]; then \
		docker rmi $$(docker images -qa); \
	fi

.PHONY: build up down re clean fclean